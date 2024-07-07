using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Timespace.Api.Database;
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
		o => o.SuppressInferBindingSourcesForParameters = true
	);

	_ = builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
		o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter())
	);

	builder.Services.AddDatabase(builder.Configuration["DbContextOptions:ConnectionString"] ?? throw new ArgumentException("Cannot find connection string"));
	builder.Services.ConfigureImmediatePlatform();

	_ = builder.Services.AddDistributedMemoryCache();
	_ = builder.Services.AddHttpContextAccessor();
	_ = builder.Services.AddSwagger();
	_ = builder.Services.AddProblemDetails(StartupExtensions.ConfigureProblemDetails);

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		_ = app.UseSwagger();
		_ = app.UseSwaggerUI();
	}

	_ = app.UseHttpsRedirection();

	_ = app.UseAuthorization();

	_ = app.MapControllers();

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
