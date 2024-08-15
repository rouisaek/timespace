using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Tenants.Members.Invites.Exceptions;
using Timespace.Api.Features.Tenants.Models;
using Timespace.Api.Features.Timesheet.Endpoints;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/accounts/register/invite")]
[AllowAnonymous]
public static partial class RegisterFromInviteCodeEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public required string InviteCode { get; set; } = null!;
		public required string FirstName { get; set; }
		public string? MiddleName { get; set; }
		public string? LastName { get; set; }
		public required string Password { get; set; }
	}

	public record Response
	{

	}

	private static async ValueTask<Response> HandleAsync(Command command, AppDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IClock clock, CancellationToken token)
	{
		var invite = await db.Invites
			.Where(x => x.Token == command.InviteCode)
			.FirstOrDefaultAsync(token) ?? throw new NotFoundException("Invite");

		var existingUser = await userManager.FindByEmailAsync(invite.Email);

		if (existingUser is not null)
			throw new UserAlreadyExistsException();

		var user = new ApplicationUser
		{
			Email = invite.Email.ToLowerInvariant(),
			EmailConfirmed = true,
			UserName = invite.Email.ToLowerInvariant(),
			FirstName = command.FirstName,
			MiddleName = command.MiddleName ?? invite.MiddleName,
			LastName = command.LastName ?? invite.LastName,
			Memberships = [new TenantUser
			{
				TenantId = invite.TenantId,
				EmployeeCode = invite.EmployeeCode,
				Permissions =
				[
					AddTimesheetEntryEndpointPolicy.PolicyName,
					GetTimesheetEntriesEndpointPolicy.PolicyName
				],
				LastLogin = clock.GetCurrentInstant()
			}]
		};

		var result = await userManager.CreateAsync(user, command.Password);

		if (result.Succeeded)
		{
			invite.IsAccepted = true;
			_ = await db.SaveChangesAsync(token);

			await signInManager.SignInAsync(user, new AuthenticationProperties());
		}
		else
		{
			throw new BadRequestException("Failed to create user.");
		}

		return new Response();
	}
}

