using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;
using Timespace.Api.Database.Common;
using Timespace.Api.Features.Tenants.Models;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Time.Models;

public enum TimesheetEntryStatus
{
	AwaitingApproval = 0,
	Denied = 1,
	Approved = 2
}

public class TimesheetEntry : ISoftDeletable, ITimestamped, IUsered, ITenanted
{
	public int TimesheetEntryId { get; set; }
	public Instant? DeletedAt { get; set; }
	public Instant CreatedAt { get; set; }
	public Instant UpdatedAt { get; set; }
	public required int UserId { get; set; }
	public ApplicationUser User { get; set; } = null!;
	public Tenant Tenant { get; set; } = null!;
	public int TenantId { get; set; }

	public required Instant ShiftStart { get; set; }
	public required Instant ShiftEnd { get; set; }
	public required Period BreakTime { get; set; } = null!;
	public required string TimeZoneId { get; set; } = null!;
	public TimesheetEntryStatus Status { get; set; } = TimesheetEntryStatus.AwaitingApproval;
	public string? DenialReason { get; set; }

	public class Configuration : IEntityTypeConfiguration<TimesheetEntry>
	{
		public void Configure(EntityTypeBuilder<TimesheetEntry> builder)
		{
			_ = builder.Property(e => e.TimeZoneId).HasMaxLength(255);
			_ = builder.Property(e => e.DenialReason).HasMaxLength(2000);
		}
	}

}
