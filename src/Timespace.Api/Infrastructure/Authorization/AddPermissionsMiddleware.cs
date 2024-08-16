using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;
using Timespace.Api.Infrastructure.Exceptions;
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

		if (context.User is { Identity.IsAuthenticated: true } claimsPrincipal)
		{
			var userId = userManager.GetUserId(claimsPrincipal) ?? throw new UnauthorizedException();

			if (int.TryParse(userId, out var userIntId))
			{
				var user = await db.Users.IgnoreQueryFilters().Include(x => x.Memberships)
					.FirstOrDefaultAsync(x => x.Id == userIntId);

				if (user is null)
					throw new UnauthorizedException();


				var firstMembership = user.Memberships.FirstOrDefault();
				usageContext.User = firstMembership ??
				                    throw new TimespaceException(500, "User has no memberships",
					                    "user-has-no-memberships");

				usageContext.TenantId = firstMembership.TenantId;

				claimsPrincipal.AddIdentity(
					new ClaimsIdentity(firstMembership.Permissions.Select(x => new Claim(Claims.Permission, x))));
			}
		}

		await next(context);
	}
}
