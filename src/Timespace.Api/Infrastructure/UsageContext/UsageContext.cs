using Timespace.Api.Features.Tenants.Models;

namespace Timespace.Api.Infrastructure.UsageContext;

public interface IUsageContext
{
	public TenantUser User { get; set; }
	public int? TenantId { get; set; }
}

[RegisterScoped]
public class UsageContext : IUsageContext
{
	public TenantUser User { get; set; } = null!;
	public int? TenantId { get; set; }
}
