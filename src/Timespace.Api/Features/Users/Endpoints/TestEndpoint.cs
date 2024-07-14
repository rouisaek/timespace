using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Timespace.Api.Database;

namespace Timespace.Api.Features.Users.Endpoints;

[GeneratePermissionPolicy]
public static class TestEndpointPolicy
{
	public const string PolicyName = "timespace:test";
}

[Handler]
[MapGet("/api/test")]
[Authorize(TestEndpointPolicy.PolicyName)]
public static partial class TestEndpoint
{
	[Validate]
	public partial record Query : IValidationTarget<Query>
	{

	}

	public record Response
	{

	}

	private static ValueTask<Response> HandleAsync(Query command, AppDbContext db, IHttpContextAccessor httpContextAccessor, CancellationToken token)
	{
		var user = httpContextAccessor.HttpContext?.User;

		return ValueTask.FromResult(new Response());
	}
}

