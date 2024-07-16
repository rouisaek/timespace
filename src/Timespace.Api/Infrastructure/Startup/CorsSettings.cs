namespace Timespace.Api.Infrastructure.Startup;

[ConfigureOptions]
public class CorsSettings
{
	public List<string> CorsDomains { get; init; } = [];
}
