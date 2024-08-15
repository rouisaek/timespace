using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Timespace.Api.Database.Common;

namespace Timespace.Api.Features.Tenants.Models;

public class Tenant : ISoftDeletable, ITimestamped
{
	public int Id { get; set; }
	public Instant? DeletedAt { get; set; }
	public Instant CreatedAt { get; set; }
	public Instant? UpdatedAt { get; set; }
	public string DisplayName { get; init; } = null!;

	public ICollection<TenantUser> TenantUsers { get; init; } = null!;

	public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
	{
		public void Configure(EntityTypeBuilder<Tenant> builder)
		{
			_ = builder.Property(x => x.DisplayName).HasMaxLength(256);
		}
	}

}

