using Destructurama.Attributed;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Validations;
using Timespace.Api.Features.Users.Models;
using Timespace.Api.Infrastructure;
using Timespace.Api.Infrastructure.Email;
using Timespace.Api.Infrastructure.Startup;

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
		EmailService emailService,
		IOptionsSnapshot<SiteSettings> siteSettings,
		CancellationToken token)
	{
		var transaction = await dbContext.Database.BeginTransactionAsync(token);
		var tenant = dbContext.Tenants.Add(new() { DisplayName = command.TenantName, });
		_ = await dbContext.SaveChangesAsync(token);

		var user = new ApplicationUser()
		{
			Email = command.Email,
			UserName = command.Email,
			TenantId = tenant.Entity.Id,
			FirstName = command.FirstName
		};

		var result = await userManager.CreateAsync(user, command.Password);

		if (result.Succeeded)
		{
			await transaction.CommitAsync(token);

			var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

			await emailService.SendConfirmEmail(
				new SendEmailProps(command.FirstName, $"{siteSettings.Value.FrontendSiteUrl}/email-confirmation/{emailConfirmationToken}?email={command.Email}"), command.Email);

			return new()
			{
				Success = true
			};
		}

		await transaction.RollbackAsync(token);
		return new() { Success = false, };

	}
}
