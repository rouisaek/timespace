using System.Globalization;
using Destructurama;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Infrastructure.Logging;

public static class LoggingStartupExtensions
{
	public static void ConfigureSerilog(this IHostBuilder host)
	{
		_ = host.UseSerilog((_, lc) => lc
			.Destructure.UsingAttributes()
			.MinimumLevel.Verbose()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
			.Filter.ByExcluding(x => x.Exception is TimespaceException && x.Properties["SourceContext"].ToString().Contains("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", StringComparison.InvariantCultureIgnoreCase))
			.Enrich.FromLogContext()
			.Enrich.WithEnvironmentName()
			.Enrich.WithThreadId()
			.Enrich.WithProperty("ExecutionId", Guid.NewGuid())
			.Enrich.WithProperty("Commit", ThisAssembly.Git.Commit)
			.Enrich.WithExceptionDetails(
				new DestructuringOptionsBuilder()
					.WithDefaultDestructurers()
					.WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
			)
			.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
			.WriteTo.Seq(serverUrl: "http://localhost:5341/")
		);
	}

	public static IApplicationBuilder UseLogging(this IApplicationBuilder app) =>
		app.UseSerilogRequestLogging(o =>
		{
			o.GetLevel = static (httpContext, _, _) =>
				httpContext.Response.StatusCode >= 500 ? LogEventLevel.Error :
				httpContext.Request.Path.StartsWithSegments(new("/api"), StringComparison.OrdinalIgnoreCase) ? LogEventLevel.Information :
				LogEventLevel.Verbose;

			o.EnrichDiagnosticContext = static (diagnosticContext, httpContext) =>
			{
				diagnosticContext.Set("User", httpContext.User?.Identity?.Name);
				diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
				diagnosticContext.Set("ConnectingIP", httpContext.Request.Headers["CF-Connecting-IP"]);
			};
		});
}
