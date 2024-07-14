using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timespace.Api.Database.Common;
using Timespace.Api.Features.Tenants.Models;

namespace Timespace.Api.Features.Users.Models;

public class ApplicationUser : IdentityUser<int>, ITenanted
{
	public Tenant Tenant { get; set; } = null!;
	public int TenantId { get; set; }
	public string FirstName { get; set; } = null!;
	public string? MiddleName { get; set; }
	public string LastName { get; set; } = null!;
	public List<string> Permissions { get; set; } = [];

	public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			_ = builder.Property(x => x.FirstName).HasMaxLength(256);
			_ = builder.Property(x => x.MiddleName).HasMaxLength(256);
			_ = builder.Property(x => x.LastName).HasMaxLength(256);
			_ = builder.Property(x => x.Permissions).HasDefaultValue(new List<string>());
		}
	}
}
