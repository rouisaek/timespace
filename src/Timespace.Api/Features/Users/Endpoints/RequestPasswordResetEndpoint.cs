using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Features.Shared.Validations;
using Timespace.Api.Features.Users.Handlers;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/accounts/password-reset/request")]
[AllowAnonymous]
public static partial class RequestPasswordResetEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress]
		public string Email { get; set; } = null!;
	}

	public record Response;

	private static async ValueTask<Response> HandleAsync(Command command, UserManager<ApplicationUser> userManager, SendPasswordResetEmail.Handler sendPasswordResetEmail, CancellationToken token)
	{
		var user = await userManager.FindByEmailAsync(command.Email);

		if (user is null)
			return new Response();

		var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);

		_ = await sendPasswordResetEmail.HandleAsync(
			new SendPasswordResetEmail.Command
			{
				Email = command.Email,
				PasswordResetToken = passwordResetToken,
				FirstName = user.FirstName
			}, token);

		return new Response();
	}
}

