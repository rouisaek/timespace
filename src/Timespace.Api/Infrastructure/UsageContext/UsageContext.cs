using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Infrastructure.UsageContext;

public interface IUsageContext
{
	public ApplicationUser User { get; set; }
	public int? TenantId { get; set; }
}

[RegisterScoped]
public class UsageContext : IUsageContext
{
	public ApplicationUser User { get; set; } = null!;
	public int? TenantId { get; set; }
}
