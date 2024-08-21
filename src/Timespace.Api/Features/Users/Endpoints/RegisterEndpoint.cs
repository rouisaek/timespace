using Destructurama.Attributed;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Validations;
using Timespace.Api.Features.Tenants.Models;
using Timespace.Api.Features.Users.EmailConfirmation.Handlers;
using Timespace.Api.Features.Users.Models;
using Timespace.Api.Infrastructure.Authorization;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/accounts/register")]
[AllowAnonymous]
public static partial class RegisterEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public required string TenantName { get; init; }
		public required string FirstName { get; init; } = null!;
		public string? MiddleName { get; init; } = null!;
		public string? LastName { get; init; } = null!;
		[EmailAddress]
		public required string Email { get; init; }
		[LogMasked]
		public required string Password { get; init; }
	}

	public record Response
	{
		public bool Success { get; set; }
	}

	private static async ValueTask<Response> HandleAsync(Command command,
		UserManager<ApplicationUser> userManager,
		AppDbContext dbContext,
		SendConfirmationEmail.Handler sendConfirmationEmailCommand,
		IUsageContext usageContext,
		CancellationToken token)
	{
		var transaction = await dbContext.Database.BeginTransactionAsync(token);
		var tenant = dbContext.Tenants.Add(new() { DisplayName = command.TenantName, });
		_ = await dbContext.SaveChangesAsync(token);

		usageContext.TenantId = tenant.Entity.Id;

		var user = new ApplicationUser()
		{
			Email = command.Email,
			UserName = command.Email,
			FirstName = command.FirstName,
			MiddleName = command.MiddleName,
			LastName = command.LastName,
			Memberships = [
				new TenantUser
				{
					TenantId = tenant.Entity.Id,
					Permissions = [AdminPolicy.PolicyName]
				}
			]
		};

		var result = await userManager.CreateAsync(user, command.Password);

		if (result.Succeeded)
		{
			await transaction.CommitAsync(token);

			var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

			_ = await sendConfirmationEmailCommand.HandleAsync(new()
			{
				Email = command.Email,
				EmailVerificationToken = confirmationToken,
				FirstName = command.FirstName,
				UserId = user.Id
			}, token);

			return new()
			{
				Success = true
			};
		}

		await transaction.RollbackAsync(token);
		return new() { Success = false, };

	}
}
