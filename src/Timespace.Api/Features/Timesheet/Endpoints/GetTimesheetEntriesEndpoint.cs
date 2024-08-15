using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Features.Timesheet.Models;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Timesheet.Endpoints;

[GeneratePermissionPolicy]
public static class GetTimesheetEntriesEndpointPolicy
{
	public const string PolicyName = "timespace:timesheet:view";
}

[Handler]
[MapPost("/api/timesheet/query")]
[Authorize(GetTimesheetEntriesEndpointPolicy.PolicyName)]
public static partial class GetTimesheetEntriesEndpoint
{
	[Validate]
	public partial record Query : IValidationTarget<Query>
	{
		public required LocalDate FromDate { get; set; }
		public required LocalDate ToDate { get; set; }
	}

	public record Response
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

	private static async ValueTask<IReadOnlyList<Response>> HandleAsync(Query command, AppDbContext db, IUsageContext usageContext, CancellationToken token)
	{
		var fromInstant = command.FromDate.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();
		var toInstant = command.ToDate.PlusDays(1).AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();

		var entries = await db.TimesheetEntries
			.Where(x => x.UserId == usageContext.User.Id)
			.Where(x => x.ShiftStart >= fromInstant && x.ShiftStart < toInstant)
			.OrderByDescending(x => x.ShiftStart)
			.Select(
			x => new Response()
			{
				Id = x.TimesheetEntryId,
				ShiftStart = x.ShiftStart,
				ShiftEnd = x.ShiftEnd,
				BreakTime = x.BreakTime,
				TimeZoneId = x.TimeZoneId,
				Status = x.Status,
				DenialReason = x.DenialReason,
				CreatedAt = x.CreatedAt,
				UpdatedAt = x.UpdatedAt,
			})
			.ToListAsync(token);

		return entries;
	}
}

