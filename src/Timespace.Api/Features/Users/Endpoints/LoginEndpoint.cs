using Destructurama.Attributed;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/login")]
[AllowAnonymous]
public static partial class LoginEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
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
			throw new BadRequestException("Email or password incorrect");

		var signInResult = await signInManager.PasswordSignInAsync(user, command.Password, command.RememberMe, false);

		if (!signInResult.Succeeded)
			throw new BadRequestException("Email or password incorrect");

		return new()
		{
			Success = true
		};
	}
}
