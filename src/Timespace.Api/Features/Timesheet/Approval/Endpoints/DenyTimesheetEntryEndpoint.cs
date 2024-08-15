using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database;
using Timespace.Api.Features.Timesheet.Exceptions;
using Timespace.Api.Features.Timesheet.Models;

namespace Timespace.Api.Features.Timesheet.Approval.Endpoints;

[GeneratePermissionPolicy]
public static class DenyTimesheetEntryEndpointPolicy
{
	public const string PolicyName = "timespace:timesheet:approval:deny";
}

[Handler]
[MapPost("/api/timesheet/deny")]
[Authorize(DenyTimesheetEntryEndpointPolicy.PolicyName)]
public static partial class DenyTimesheetEntryEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public required int TimesheetEntryId { get; set; }
		public string? DenialReason { get; set; }
	}

	public record Response;

	private static async ValueTask<Response> HandleAsync(Command command, AppDbContext db, CancellationToken token)
	{
		var entry = await db.TimesheetEntries
			.Where(x => x.TimesheetEntryId == command.TimesheetEntryId)
			.FirstOrDefaultAsync(token);

		if (entry is null)
			throw new TimesheetEntryNotFoundException();

		entry.Status = TimesheetEntryStatus.Denied;
		entry.DenialReason = command.DenialReason;

		_ = await db.SaveChangesAsync(token);

		return new Response();
	}
}

