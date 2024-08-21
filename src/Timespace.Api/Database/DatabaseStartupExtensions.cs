using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Timespace.Api.Database;

public static class DatabaseStartupExtensions
{
	public static void AddDatabase(this IServiceCollection services, string connectionString, bool isDev = false)
	{
		var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
		_ = dataSourceBuilder.UseNodaTime();
		var dataSource = dataSourceBuilder.Build();

		_ = services.AddDbContext<AppDbContext>(c =>
		{
			_ = c.UseNpgsql(dataSource, o => o.UseNodaTime());
			if (isDev)
				_ = c.EnableSensitiveDataLogging();
		});
	}

	public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		_ = db.Database.EnsureCreated();

		db.Database.Migrate();
		return app;
	}
}
