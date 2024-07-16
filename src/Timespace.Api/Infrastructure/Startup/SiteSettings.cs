using System.Diagnostics.CodeAnalysis;

namespace Timespace.Api.Infrastructure.Startup;

[ConfigureOptions]
public class SiteSettings
{
	[SuppressMessage("Design", "CA1056:URI-like properties should not be strings")]
	public string FrontendSiteUrl { get; set; } = null!;
}
