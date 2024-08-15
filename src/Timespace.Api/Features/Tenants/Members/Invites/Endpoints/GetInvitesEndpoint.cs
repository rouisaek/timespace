using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;

namespace Timespace.Api.Features.Tenants.Members.Invites.Endpoints;

[GeneratePermissionPolicy]
public static class GetInvitesEndpointPolicy
{
	public const string PolicyName = "timespace:tenant:members:invites:view";
}

[Handler]
[MapGet("/api/tenant/members/invites")]
[Authorize(GetInvitesEndpointPolicy.PolicyName)]
public static partial class GetInvitesEndpoint
{
	public record Query;

	public record Response
	{
		public required int InviteId { get; set; }
		public required string Email { get; set; } = null!;
		public required string Token { get; set; } = null!;
		public required Instant ExpiresAt { get; set; }
		public required Instant CreatedAt { get; set; }
		public required Instant? UpdatedAt { get; set; }
	}

	private static async ValueTask<IReadOnlyList<Response>> HandleAsync(Query _, AppDbContext db, CancellationToken token)
	{
		var invites = await db.Invites
			.Where(x => !x.IsAccepted)
			.Select(x => new Response
			{
				InviteId = x.InviteId,
				Email = x.Email.ToLowerInvariant(),
				Token = x.Token,
				ExpiresAt = x.ExpiresAt,
				CreatedAt = x.CreatedAt,
				UpdatedAt = x.UpdatedAt,
			}).ToListAsync(token);

		return invites;
	}
}

