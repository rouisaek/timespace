namespace Timespace.Api.Infrastructure.Middleware;

[RegisterSingleton(typeof(AddRequestIdHeaderMiddleware))]
public class AddRequestIdHeaderMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		context.Response.Headers.Append("RequestId", context.TraceIdentifier);
		await next(context);
	}
}
