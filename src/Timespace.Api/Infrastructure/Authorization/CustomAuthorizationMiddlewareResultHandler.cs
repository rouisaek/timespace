using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Infrastructure.Authorization;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
	private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

	public async Task HandleAsync(
		RequestDelegate next,
		HttpContext context,
		AuthorizationPolicy policy,
		PolicyAuthorizationResult authorizeResult)
	{
		if (authorizeResult.Forbidden)
		{
			// Return a 404 to make it appear as if the resource doesn't exist.
			throw new ForbiddenException();
		}

		// Fall back to the default implementation.
		await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
	}
}

public class Show404Requirement : IAuthorizationRequirement { }
