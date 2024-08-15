using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Timespace.Api.Database.Common;
using Timespace.Api.Features.Tenants.Models;

namespace Timespace.Api.Features.Tenants.Members.Invites.Models;

public class Invite : ITenanted, ITimestamped
{
	public int InviteId { get; set; }
	public Tenant Tenant { get; set; } = null!;
	public int TenantId { get; set; }
	public Instant CreatedAt { get; set; }
	public Instant? UpdatedAt { get; set; }

	public required string Email { get; set; } = null!;
	public required string Token { get; set; } = null!;
	public required Instant ExpiresAt { get; set; }
	public bool IsAccepted { get; set; }
	public string? EmployeeCode { get; set; }
	public string? FirstName { get; set; }
	public string? MiddleName { get; set; }
	public string? LastName { get; set; }

	public class Configuration : IEntityTypeConfiguration<Invite>
	{
		public void Configure(EntityTypeBuilder<Invite> builder)
		{
			_ = builder.HasIndex(x => x.Token).IsUnique();

			_ = builder.Property(e => e.Email).HasMaxLength(256);
			_ = builder.Property(e => e.Token).HasMaxLength(256);
			_ = builder.Property(e => e.EmployeeCode).HasMaxLength(256);
			_ = builder.Property(e => e.FirstName).HasMaxLength(256);
			_ = builder.Property(e => e.MiddleName).HasMaxLength(256);
			_ = builder.Property(e => e.LastName).HasMaxLength(256);
		}
	}
}
