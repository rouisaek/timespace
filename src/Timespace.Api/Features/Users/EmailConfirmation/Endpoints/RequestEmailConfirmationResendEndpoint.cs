using System.ComponentModel.DataAnnotations;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Features.Users.EmailConfirmation.Handlers;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.EmailConfirmation.Endpoints;

[Handler]
[MapPost("/api/accounts/email-confirmation/resend")]
[AllowAnonymous]
public static partial class RequestEmailConfirmationResendEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress] public required string Email { get; init; } = null!;
	}

	public record Response
	{
		public bool Success { get; set; }
	}

	private static async ValueTask<Response> HandleAsync(Command command,
		UserManager<ApplicationUser> userManager,
		SendConfirmationEmailCommand.Handler sendConfirmationEmailCommand,
		CancellationToken token)
	{
		var user = await userManager.FindByEmailAsync(command.Email);

		if (user is null)
		{
			return new Response { Success = true };
		}

		_ = await sendConfirmationEmailCommand.HandleAsync(new()
		{
			Email = user.Email!,
			FirstName = user.FirstName,
			User = user
		}, token);

		return new Response { Success = true }; ;
	}
}
