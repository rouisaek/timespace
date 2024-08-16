using System.ComponentModel.DataAnnotations;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Features.Users.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/accounts/password-reset/reset")]
[AllowAnonymous]
public static partial class ResetPasswordEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress] public string Email { get; set; } = null!;
		public string Token { get; set; } = null!;
		public string Password { get; set; } = null!;
	}

	public record Response;

	private static async ValueTask<Response> HandleAsync(
		Command command,
		UserManager<ApplicationUser> userManager,
		CancellationToken _)
	{
		var user = await userManager.FindByEmailAsync(command.Email);

		if (user is null)
			throw new InvalidPasswordResetTokenException();

		var result = await userManager.ResetPasswordAsync(user, command.Token, command.Password);

		if (!result.Succeeded)
			throw new InvalidPasswordResetTokenException();

		return new Response();
	}
}
