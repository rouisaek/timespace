using System.ComponentModel.DataAnnotations;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Features.Users.EmailConfirmation.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.EmailConfirmation.Endpoints;

[Handler]
[MapPost("/api/accounts/email-confirmation")]
[AllowAnonymous]
public static partial class ConfirmEmailEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress]
		public string Email { get; init; } = null!;
		public string Code { get; init; } = null!;
	}

	public record Response
	{
		public bool Success { get; set; }
	}

	private static async ValueTask<Response> HandleAsync(Command command, UserManager<ApplicationUser> userManager, CancellationToken token)
	{
		var user = await userManager.FindByEmailAsync(command.Email);

		if (user is null)
			throw new EmailConfirmationFailedException();

		var result = await userManager.ConfirmEmailAsync(user, command.Code);

		if (!result.Succeeded)
			throw new EmailConfirmationFailedException();

		return new Response { Success = true };
	}
}

