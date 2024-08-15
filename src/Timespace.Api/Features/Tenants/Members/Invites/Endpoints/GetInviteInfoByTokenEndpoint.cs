using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Features.Tenants.Members.Invites.Endpoints;

[Handler]
[MapGet("/api/tenant/members/invites/{token}")]
[AllowAnonymous]
public static partial class GetInviteInfoByTokenEndpoint
{
	[Validate]
	public partial record Query : IValidationTarget<Query>
	{
		[FromRoute(Name = "token")]
		public string Token { get; set; } = null!;
	}

	public record Response
	{
		public required string Email { get; init; } = null!;
		public required string? FirstName { get; init; }
		public required string? MiddleName { get; init; }
		public required string? LastName { get; init; }
	}

	private static async ValueTask<Response> HandleAsync(Query command, AppDbContext db, CancellationToken token)
	{
		return await db.Invites
			.Where(x => x.Token == command.Token && x.IsAccepted == false)
			.Select(x => new Response
			{
				Email = x.Email.ToLowerInvariant(),
				FirstName = x.FirstName,
				MiddleName = x.MiddleName,
				LastName = x.LastName
			})
			.FirstOrDefaultAsync(token) ?? throw new NotFoundException("Invite");
	}
}

