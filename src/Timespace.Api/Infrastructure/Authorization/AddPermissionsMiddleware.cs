using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Infrastructure.Authorization;

[RegisterSingleton(typeof(AddPermissionsMiddleware))]
public class AddPermissionsMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		using var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
		await using var db = context.RequestServices.GetRequiredService<AppDbContext>();
		var usageContext = context.RequestServices.GetRequiredService<IUsageContext>();

		if (context.User is { } claimsPrincipal)
		{
			var userId = userManager.GetUserId(claimsPrincipal) ?? throw new UnauthorizedException();
			var user = await userManager.FindByIdAsync(userId) ?? throw new UnauthorizedException();

			usageContext.User = user;

			claimsPrincipal.AddIdentity(new ClaimsIdentity(user.Permissions.Select(x => new Claim(Claims.Permission, x))));
		}

		await next(context);
	}
}
