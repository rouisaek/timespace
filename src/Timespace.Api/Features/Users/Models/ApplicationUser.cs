using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Timespace.Api.Database.Common;
using Timespace.Api.Features.Tenants.Models;

// using Timespace.Api.Features.Tenants.Models;

namespace Timespace.Api.Features.Users.Models;

public class ApplicationUser : IdentityUser<int>, ITimestamped
{
	public string FirstName { get; set; } = null!;
	public string? MiddleName { get; set; }
	public string? LastName { get; set; }
	public new required string Email
	{
		get => base.Email ?? throw new InvalidOperationException("Email should not be null");
		set => base.Email = value;
	}
	public Instant? LastEmailConfirmationSent { get; set; }
	public ICollection<TenantUser> Memberships { get; init; } = null!;

	public Instant CreatedAt { get; set; }
	public Instant? UpdatedAt { get; set; }

	public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			_ = builder.Property(x => x.FirstName).HasMaxLength(256);
			_ = builder.Property(x => x.MiddleName).HasMaxLength(256);
			_ = builder.Property(x => x.LastName).HasMaxLength(256);
			_ = builder.Property(x => x.Email).HasMaxLength(256);
		}
	}
}
