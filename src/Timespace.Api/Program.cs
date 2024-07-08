using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Serilog;
using Timespace.Api.Database;
using Timespace.Api.Infrastructure.Logging;
using Timespace.Api.Infrastructure.Middleware;
using Timespace.Api.Infrastructure.Startup;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console(formatProvider: null)
	.CreateBootstrapLogger();
try
{
	var builder = WebApplication.CreateBuilder(args);

	_ = builder.Configuration.AddJsonFile("secrets.json", optional: true);

	// Add services to the container.

	builder.Host.ConfigureSerilog();

	_ = builder.Services.Configure<ApiBehaviorOptions>(
		o =>
		{
			o.SuppressInferBindingSourcesForParameters = true;
		}
	);

	_ = builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

	_ = builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
		o =>
		{
			o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
			_ = o.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
		}
	);

	builder.Services.AddDatabase(builder.Configuration["DbContextOptions:ConnectionString"] ?? throw new ArgumentException("Cannot find connection string"));
	builder.Services.ConfigureImmediatePlatform();
	builder.Services.AddIdentity();
	// builder.Services.AddHellangProblemDetails();

	_ = builder.Services.AddDistributedMemoryCache();
	_ = builder.Services.AddHttpContextAccessor();
	_ = builder.Services.AddAuthorization();
	_ = builder.Services.AddEndpointsApiExplorer();
	_ = builder.Services.AddSwagger();
	_ = builder.Services.AddProblemDetails(StartupExtensions.ConfigureProblemDetails);
	_ = builder.Services.AddSingleton<AddRequestIdHeaderMiddleware>();

	_ = builder.Services.AddResponseCompression(
		options => options.EnableForHttps = true
	);

	var app = builder.Build();

	_ = app.UseExceptionHandler();
	_ = app.UseMiddleware<AddRequestIdHeaderMiddleware>();
	_ = app.UseHttpsRedirection();
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
	_ = app.UseRouting();
	_ = app.UseAuthorization();
	_ = app.UseLogging();

	_ = app.UseEndpoints(
		endpoints =>
		{
			_ = endpoints.MapTimespaceApiEndpoints();
		}
	);

	await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
	Log.Fatal(ex, "Unhandled exception");
}
finally
{
	if (new StackTrace().FrameCount == 1)
	{
		Log.Information("Shut down complete");
		await Log.CloseAndFlushAsync();
	}
}
