using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Timespace.Api.Database;

public class DesignTimeDbContext : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("secrets.json")
			.Build();

		var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration["DbContextOptions:ConnectionString"]);
		_ = dataSourceBuilder.UseNodaTime();
		var dataSource = dataSourceBuilder.Build();

		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
		_ = optionsBuilder.UseNpgsql(dataSource, opt =>
		{
			_ = opt.UseNodaTime();
		});

		return new AppDbContext(optionsBuilder.Options);
	}
}
