using Microsoft.AspNetCore.Identity;
using Timespace.Api.Database.EntityAttributes;
using Timespace.Api.Features.Tenants.Models;

namespace Timespace.Api.Features.Users.Models;

public class ApplicationUser : IdentityUser<int>, ITenanted
{
	public Tenant Tenant { get; set; } = null!;
	public int TenantId { get; set; }
}
