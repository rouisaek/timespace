using NodaTime;
using Timespace.Api.Features.Timesheet.Models;

namespace Timespace.Api.Features.Timesheet.Endpoints;

public class TimesheetEntryDto
{
	public required int Id { get; set; }
	public required Instant ShiftStart { get; set; }
	public required Instant ShiftEnd { get; set; }
	public required Period BreakTime { get; set; } = null!;
	public required string TimeZoneId { get; set; } = null!;
	public required TimesheetEntryStatus Status { get; set; } = TimesheetEntryStatus.AwaitingApproval;
	public required string? DenialReason { get; set; }
	public required Instant CreatedAt { get; set; }
	public required Instant? UpdatedAt { get; set; }
}
