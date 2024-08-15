using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Timesheet.Models;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Timesheet.Endpoints;

[GeneratePermissionPolicy]
public static class AddTimesheetEntryEndpointPolicy
{
	public const string PolicyName = "timespace:timesheet:add";
}

[Handler]
[MapPost("/api/timesheet")]
[Authorize(AddTimesheetEntryEndpointPolicy.PolicyName)]
public static partial class AddTimesheetEntryEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public Instant ShiftStart { get; set; }
		public Instant ShiftEnd { get; set; }
		public Period BreakTime { get; set; } = null!;
		public string TimeZoneId { get; set; } = null!;
	}

	public record Response
	{

	}

	private static async ValueTask<Response> HandleAsync(Command command, AppDbContext db, IUsageContext usageContext, CancellationToken token)
	{
		if (command.ShiftStart > command.ShiftEnd)
		{
			throw new BadRequestException("Shift start must be before shift end", "shift-start-must-be-before-shift-end");
		}

		if (command.ShiftEnd.Minus(command.ShiftStart).Minus(command.BreakTime.ToDuration()) < Duration.Zero)
		{
			throw new BadRequestException("Shift duration must be positive", "shift-duration-must-be-positive");
		}

		var entry = new TimesheetEntry
		{
			ShiftStart = command.ShiftStart,
			ShiftEnd = command.ShiftEnd,
			BreakTime = command.BreakTime,
			TimeZoneId = command.TimeZoneId,
			UserId = usageContext.User.Id,
		};

		_ = db.TimesheetEntries.Add(entry);
		_ = await db.SaveChangesAsync(token);

		await Task.Run(() => { }, token);
		return new Response();
	}
}

