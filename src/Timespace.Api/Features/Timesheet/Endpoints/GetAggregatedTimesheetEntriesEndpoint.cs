using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Timespace.Api.Database;

namespace Timespace.Api.Features.Timesheet.Endpoints;

[GeneratePermissionPolicy]
public static class GetAggregatedTimesheetEntriesEndpointPolicy
{
	public const string PolicyName = "timespace:timesheet:aggregated:view";
}

[Handler]
[MapPost("/api/timesheet/aggregated")]
[Authorize(GetAggregatedTimesheetEntriesEndpointPolicy.PolicyName)]
public static partial class GetAggregatedTimesheetEntriesEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public required LocalDate FromDate { get; set; }
		public required LocalDate ToDate { get; set; }
	}

	public record Response
	{
		public required int TenantUserId { get; init; }
		public required string FirstName { get; init; }
		public string? MiddleName { get; init; }
		public string? LastName { get; init; }
		public List<TimesheetEntryDto> Entries { get; init; } = [];
	}

	private static async ValueTask<IReadOnlyList<Response>> HandleAsync(
		Command command,
		AppDbContext db,
		CancellationToken token)
	{
		var fromInstant = command.FromDate.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();
		var toInstant = command.ToDate.PlusDays(1).AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();

		var responseList = await db.TenantUsers
			.Select(
				x => new Response
				{
					TenantUserId = x.Id,
					FirstName = x.User.FirstName,
					MiddleName = x.User.MiddleName,
					LastName = x.User.LastName,
					Entries = x.TimesheetEntries
						.Where(y => y.ShiftStart >= fromInstant && y.ShiftStart < toInstant)
						.OrderByDescending(y => y.ShiftStart)
						.Select(
							y => new TimesheetEntryDto
							{
								Id = y.TimesheetEntryId,
								ShiftStart = y.ShiftStart,
								ShiftEnd = y.ShiftEnd,
								BreakTime = y.BreakTime,
								TimeZoneId = y.TimeZoneId,
								Status = y.Status,
								DenialReason = y.DenialReason,
								CreatedAt = y.CreatedAt,
								UpdatedAt = y.UpdatedAt
							})
						.ToList()
				})
			.ToListAsync(token);

		return responseList.Where(x => x.Entries.Count != 0).ToList();
	}
}
