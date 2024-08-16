using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;
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

	private static async ValueTask<IReadOnlyList<TimesheetEntryDto>> HandleAsync(
		Query command,
		AppDbContext db,
		IUsageContext usageContext,
		CancellationToken token)
	{
		var fromInstant = command.FromDate.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();
		var toInstant = command.ToDate.PlusDays(1).AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();

		var entries = await db.TimesheetEntries
			.Where(x => x.TenantUserId == usageContext.User.Id)
			.Where(x => x.ShiftStart >= fromInstant && x.ShiftStart < toInstant)
			.OrderByDescending(x => x.ShiftStart)
			.Select(
				x => new TimesheetEntryDto
				{
					Id = x.TimesheetEntryId,
					ShiftStart = x.ShiftStart,
					ShiftEnd = x.ShiftEnd,
					BreakTime = x.BreakTime,
					TimeZoneId = x.TimeZoneId,
					Status = x.Status,
					DenialReason = x.DenialReason,
					CreatedAt = x.CreatedAt,
					UpdatedAt = x.UpdatedAt
				})
			.ToListAsync(token);

		return entries;
	}
}
