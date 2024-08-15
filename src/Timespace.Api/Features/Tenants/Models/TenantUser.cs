using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Timespace.Api.Database.Common;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Tenants.Models;

public class TenantUser : ITenanted, IUsered, ITimestamped
{
	public int Id { get; set; }
	public int TenantId { get; set; }
	public Tenant Tenant { get; set; } = null!;
	public int UserId { get; set; }
	public ApplicationUser User { get; set; } = null!;

	public List<string> Permissions { get; init; } = null!;
	public Instant? LastLogin { get; set; }
	public string? EmployeeCode { get; set; }

	public Instant CreatedAt { get; set; }
	public Instant? UpdatedAt { get; set; }

	public class Configuration : IEntityTypeConfiguration<TenantUser>
	{
		public void Configure(EntityTypeBuilder<TenantUser> builder)
		{
			_ = builder.Property(e => e.EmployeeCode).HasMaxLength(255);
		}
	}
}
