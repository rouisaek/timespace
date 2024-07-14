using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Infrastructure.Authorization;

[RegisterSingleton(typeof(AddPermissionsMiddleware))]
public class AddPermissionsMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		using var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
		await using var db = context.RequestServices.GetRequiredService<AppDbContext>();

		if (context.User is { } claimsPrincipal)
		{
			var userId = userManager.GetUserId(claimsPrincipal);

			if (int.TryParse(userId, out var userIdInt))
			{
				var user = await db.Users.FindAsync(userIdInt) ?? throw new UnauthorizedException();
				claimsPrincipal.AddIdentity(new ClaimsIdentity(user.Permissions.Select(x => new Claim(Claims.Permission, x))));
			}
		}

		await next(context);
	}
}
