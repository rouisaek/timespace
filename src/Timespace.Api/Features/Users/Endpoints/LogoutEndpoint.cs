using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/accounts/logout")]
public static partial class LogoutEndpoint
{
	private static async ValueTask HandleAsync(object _, SignInManager<ApplicationUser> signInManager, CancellationToken __)
	{
		await signInManager.SignOutAsync();
	}
}

