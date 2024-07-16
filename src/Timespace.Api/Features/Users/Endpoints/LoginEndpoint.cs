using Destructurama.Attributed;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Shared.Validations;
using Timespace.Api.Features.Users.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/accounts/login")]
[AllowAnonymous]
public static partial class LoginEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress]
		public required string Email { get; set; }
		[LogMasked]
		public required string Password { get; set; }

		public bool RememberMe { get; set; }
	}

	public record Response
	{
		public bool Success { get; set; }
	}

	private static async ValueTask<Response> HandleAsync(Command command, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, CancellationToken _)
	{
		var user = await userManager.FindByEmailAsync(command.Email);

		if (user == null)
			throw new LoginFailedException();

		var signInResult = await signInManager.PasswordSignInAsync(user, command.Password, command.RememberMe, false);

		if (!signInResult.Succeeded)
		{
			if (signInResult.IsNotAllowed && !user.EmailConfirmed)
			{
				throw new BadRequestException("Email has not been confirmed yet.",
					"email-not-confirmed");
			}

			throw new LoginFailedException();
		}

		return new()
		{
			Success = true
		};
	}
}
