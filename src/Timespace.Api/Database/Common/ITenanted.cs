using Timespace.Api.Features.Tenants.Models;

namespace Timespace.Api.Database.Common;

public interface ITenanted
{
	public Tenant Tenant { get; set; }
	public int TenantId { get; set; }
}
