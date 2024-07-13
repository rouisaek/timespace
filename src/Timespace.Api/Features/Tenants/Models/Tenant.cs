using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timespace.Api.Database.Common;

namespace Timespace.Api.Features.Tenants.Models;

public class Tenant : IEntity
{
	public int Id { get; set; }
	public string DisplayName { get; init; } = null!;

	public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
	{
		public void Configure(EntityTypeBuilder<Tenant> builder)
		{
			_ = builder.Property(x => x.DisplayName).HasMaxLength(256);
		}
	}
}

