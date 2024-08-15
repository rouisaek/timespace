using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Tenants.Members.Endpoints;

[GeneratePermissionPolicy]
public static class GetMembersEndpointPolicy
{
	public const string PolicyName = "timespace:tenant:members:view";
}

[Handler]
[MapGet("/api/tenant/members")]
[Authorize(GetMembersEndpointPolicy.PolicyName)]
public static partial class GetMembersEndpoint
{
	public record Query;

	public record Response
	{
		public required int UserId { get; set; }
		public string? EmployeeCode { get; set; }
		public required string FirstName { get; set; } = null!;
		public string? MiddleName { get; set; }
		public string? LastName { get; set; }
		public required string Email { get; set; }
	}

	private static async ValueTask<IReadOnlyList<Response>> HandleAsync(Query _, AppDbContext db, IUsageContext usageContext, CancellationToken token)
	{
		var members = await db.TenantUsers.Select(x => new Response
		{
			UserId = x.UserId,
			EmployeeCode = x.EmployeeCode,
			FirstName = x.User.FirstName,
			MiddleName = x.User.MiddleName,
			LastName = x.User.LastName,
			Email = x.User.Email
		}).ToListAsync(token);

		return members;
	}
}

