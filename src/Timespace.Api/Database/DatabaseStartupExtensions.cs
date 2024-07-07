using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Timespace.Api.Database;

public static class DatabaseStartupExtensions
{
	public static void AddDatabase(this IServiceCollection services, string connectionString)
	{
		var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
		_ = dataSourceBuilder.UseNodaTime();
		var dataSource = dataSourceBuilder.Build();

		_ = services.AddDbContext<AppDbContext>(c =>
		{
			_ = c.UseNpgsql(dataSource, o => o.UseNodaTime());
		});
	}
}
