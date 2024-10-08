using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Serilog;
using Timespace.Api.Database;
using Timespace.Api.Infrastructure.Authorization;
using Timespace.Api.Infrastructure.Exceptions;
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
	_ = builder.Configuration.AddJsonFile("appsettings.json", optional: true);
	_ = builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
	_ = builder.Configuration.AddEnvironmentVariables();
	_ = builder.Services.ConfigureAllOptions();

	// Add services to the container.
	builder.Host.ConfigureSerilog(new Uri(builder.Configuration["SeqUrl"] ?? ""));

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
			// o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
			_ = o.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
		}
	);

	builder.Services.AddDatabase(builder.Configuration["DbContextOptions:ConnectionString"] ?? throw new ArgumentException("Cannot find connection string"), builder.Environment.EnvironmentName == "DEV");
	builder.Services.ConfigureImmediatePlatform();
	builder.Services.AddIdentity(builder.Configuration["SiteSettings:FrontendSiteUrl"] ?? throw new ArgumentException("Cannot find frontend url"));
	_ = builder.Services.AddSingleton<IClock>(SystemClock.Instance);
	_ = builder.Services.AddPermissionPolicies();

	_ = builder.Services.AddDistributedMemoryCache();
	_ = builder.Services.AddHttpContextAccessor();
	_ = builder.Services.AddEndpointsApiExplorer();
	_ = builder.Services.AddSwagger();
	_ = builder.Services.AddProblemDetails(ExceptionStartupExtensions.ConfigureProblemDetails);
	_ = builder.Services.AutoRegisterFromTimespaceApi();
	_ = builder.Services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials()
		.WithOrigins(builder.Configuration.GetSection("CorsSettings").GetSection("CorsDomains").Get<string[]>() ?? [])));

	_ = builder.Services.AddResponseCompression(
		options => options.EnableForHttps = true
	);

	var app = builder.Build();

	using (var scope = app.Services.CreateScope())
	{
		var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await db.Database.MigrateAsync();
	}

	_ = app.UseLogging();
	_ = app.UseExceptionHandler();
	_ = app.UseMiddleware<AddPermissionsMiddleware>();
	_ = app.UseMiddleware<AddRequestIdHeaderMiddleware>();
	_ = app.UseHttpsRedirection();
	_ = app.UseStaticFiles();
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
	_ = app.UseRouting();
	_ = app.UseCors();
	_ = app.UseAuthorization();

	_ = app.UseEndpoints(
		endpoints =>
		{
			_ = endpoints.MapTimespaceApiEndpoints();
			_ = endpoints.MapFallbackToFile("index.html").AllowAnonymous();
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
