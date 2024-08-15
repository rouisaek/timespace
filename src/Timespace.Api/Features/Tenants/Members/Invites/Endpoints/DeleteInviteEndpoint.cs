using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Features.Tenants.Members.Invites.Endpoints;

[GeneratePermissionPolicy]
public static class DeleteInviteEndpointPolicy
{
	public const string PolicyName = "timespace:tenant:members:invites:delete";
}

[Handler]
[MapDelete("/api/tenant/members/invites/{inviteId}")]
[Authorize(DeleteInviteEndpointPolicy.PolicyName)]
public static partial class DeleteInviteEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[FromRoute(Name = "inviteId")]
		public int InviteId { get; set; }
	}

	public record Response;

	private static async ValueTask<Response> HandleAsync(Command command, AppDbContext db, CancellationToken token)
	{
		var affected = await db.Invites
			.Where(x => x.InviteId == command.InviteId)
			.ExecuteDeleteAsync(token);

		if (affected == 0)
			throw new NotFoundException("Invite");

		return new Response();
	}
}

