using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Features.Timesheet.Models;

namespace Timespace.Api.Features.Timesheet.Approval.Endpoints;

[GeneratePermissionPolicy]
public static class ApproveAllTimesheetEntriesEndpointPolicy
{
	public const string PolicyName = "timespace:timesheet:approval:approve-all";
}

[Handler]
[MapPost("/api/timesheet/approve-all")]
[Authorize(ApproveAllTimesheetEntriesEndpointPolicy.PolicyName)]
public static partial class ApproveAllTimesheetEntriesEndpoint
{
	public record Command;

	public record Response
	{
		public required int UpdatedEntryCount { get; set; }
	}

	private static async ValueTask<Response> HandleAsync(Command command, AppDbContext db, IClock clock, CancellationToken token)
	{
		var updatedEntryCount = await db.TimesheetEntries
			.Where(x => x.Status == TimesheetEntryStatus.AwaitingApproval)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(x => x.Status, TimesheetEntryStatus.Approved)
				.SetProperty(x => x.UpdatedAt, clock.GetCurrentInstant()), token);

		return new Response
		{
			UpdatedEntryCount = updatedEntryCount
		};
	}
}

