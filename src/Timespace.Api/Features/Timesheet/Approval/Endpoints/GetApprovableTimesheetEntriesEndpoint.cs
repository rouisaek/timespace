using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Features.Timesheet.Models;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Timesheet.Approval.Endpoints;

[GeneratePermissionPolicy]
public static class GetApprovableTimesheetEntriesEndpointPolicy
{
	public const string PolicyName = "timespace:timesheet:approval:view";
}

[Handler]
[MapGet("/api/timesheet/approvable-entries")]
[Authorize(GetApprovableTimesheetEntriesEndpointPolicy.PolicyName)]
public static partial class GetApprovableTimesheetEntriesEndpoint
{
	[Validate]
	public partial record Query : IValidationTarget<Query>
	{

	}

	public record Response
	{
		public required int Id { get; set; }
		public required Instant ShiftStart { get; set; }
		public required Instant ShiftEnd { get; set; }
		public required Period BreakTime { get; set; } = null!;
		public required string TimeZoneId { get; set; } = null!;
		public required string UserName { get; set; } = null!;
		public required Instant CreatedAt { get; set; }
		public required Instant? UpdatedAt { get; set; }
	}

	private static async ValueTask<IReadOnlyList<Response>> HandleAsync(Query command, AppDbContext db, IUsageContext usageContext, CancellationToken token)
	{
		var entries = await db.TimesheetEntries
			.Where(x => x.Status == TimesheetEntryStatus.AwaitingApproval)
			.OrderByDescending(x => x.ShiftStart)
			.Select(
				x => new Response
				{
					Id = x.TimesheetEntryId,
					ShiftStart = x.ShiftStart,
					ShiftEnd = x.ShiftEnd,
					BreakTime = x.BreakTime,
					TimeZoneId = x.TimeZoneId,
					UserName = x.User.FirstName,
					CreatedAt = x.CreatedAt,
					UpdatedAt = x.UpdatedAt,
				})
			.ToListAsync(token);

		return entries;
	}
}

