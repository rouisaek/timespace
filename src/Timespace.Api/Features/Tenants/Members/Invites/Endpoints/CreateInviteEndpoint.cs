using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Shared.Helpers;
using Timespace.Api.Features.Tenants.Members.Invites.Exceptions;
using Timespace.Api.Features.Tenants.Members.Invites.Handlers;
using Timespace.Api.Features.Tenants.Members.Invites.Models;
using Timespace.Api.Features.Users.Models;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Tenants.Members.Invites.Endpoints;

[GeneratePermissionPolicy]
public static class CreateInviteEndpointPolicy
{
	public const string PolicyName = "timespace:tenant:members:invites:create";
}

[Handler]
[MapPost("/api/tenant/members/invites")]
[Authorize(CreateInviteEndpointPolicy.PolicyName)]
public static partial class CreateInviteEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public string Email { get; set; } = null!;
		public Instant ExpiresAt { get; set; }
		public string? EmployeeCode { get; set; }
		public string? FirstName { get; set; }
		public string? MiddleName { get; set; }
		public string? LastName { get; set; }
	}

	public record Response
	{

	}

	private static async ValueTask<Response> HandleAsync(Command command, AppDbContext db, UserManager<ApplicationUser> userManager, IUsageContext usageContext, SendInviteEmailToUser.Handler sendInviteEmailToUser, CancellationToken token)
	{
		var existingUser = await db.Users
			.Where(x => x.NormalizedEmail == userManager.NormalizeEmail(command.Email))
			.FirstOrDefaultAsync(token);

		if (existingUser is not null)
			throw new UserAlreadyExistsException();

		var existingInvite = await db.Invites
			.Where(x => x.Email == command.Email.ToUpperInvariant())
			.FirstOrDefaultAsync(token);

		if (existingInvite is not null)
			throw new UserAlreadyExistsException();

		var invite = new Invite
		{
			Email = command.Email.ToUpperInvariant(),
			Token = SecureRandomStringGenerator.Generate(),
			ExpiresAt = command.ExpiresAt,
			EmployeeCode = command.EmployeeCode,
			FirstName = command.FirstName,
			MiddleName = command.MiddleName,
			LastName = command.LastName
		};

		_ = db.Invites.Add(invite);

		_ = await db.SaveChangesAsync(token);

		var tenantName = await db.Tenants.Where(x => x.Id == usageContext.TenantId).Select(x => x.DisplayName)
			.FirstOrDefaultAsync(token);

		if (tenantName is null)
			throw new BadRequestException("Tenant not found");

		_ = await sendInviteEmailToUser.HandleAsync(
			new()
			{
				Email = command.Email,
				FirstName = command.FirstName ?? "",
				InviteToken = invite.Token,
				TenantName = tenantName
			}, token);

		return new Response();
	}
}

