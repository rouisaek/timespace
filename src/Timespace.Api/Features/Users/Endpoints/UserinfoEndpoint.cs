using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
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

	private static ValueTask<Response> HandleAsync(object _, IUsageContext usageContext, CancellationToken __)
	{
		var user = usageContext.User;

		return new ValueTask<Response>(new Response()
		{
			FirstName = user.FirstName,
			MiddleName = user.MiddleName,
			LastName = user.LastName,
			Email = user.Email,
			Permissions = user.Permissions
		});
	}
}

