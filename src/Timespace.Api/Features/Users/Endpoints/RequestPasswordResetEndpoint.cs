using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Validations;
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
		public string EmailAddress { get; set; } = null!;
	}

	public record Response
	{
		public bool Success { get; set; }
	}

	private static async ValueTask<Response> HandleAsync(Command command, UserManager<ApplicationUser> userManager, CancellationToken token)
	{
		var user = await userManager.FindByEmailAsync(command.EmailAddress);

		if (user is null)
			return new Response { Success = true };

		var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);



		return new Response { Success = true };
	}
}

