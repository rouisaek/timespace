using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapGet("/api/accounts/me")]
[Authorize]
public static partial class UserinfoEndpoint
{
	public record Response
	{
		public required string FirstName { get; set; }
		public string? MiddleName { get; set; }
		public string? LastName { get; set; }
		public required string Email { get; set; }
		public required List<string> Permissions { get; init; }
	}

	private static async ValueTask<Response> HandleAsync(
		object _,
		IUsageContext usageContext,
		AppDbContext db,
		CancellationToken token)
	{
		var response = await db.TenantUsers
						   .Where(x => x.Id == usageContext.User.Id)
						   .Select(x => new Response
						   {
							   FirstName = x.User.FirstName,
							   MiddleName = x.User.MiddleName,
							   LastName = x.User.LastName,
							   Email = x.User.Email,
							   Permissions = x.Permissions
						   })
						   .FirstOrDefaultAsync(token) ??
					   throw new BadRequestException("Something went wrong with projecting into userinfo object");
		return response;

	}
}
