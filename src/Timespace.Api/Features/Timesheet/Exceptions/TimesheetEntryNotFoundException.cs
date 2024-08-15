using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Features.Timesheet.Exceptions;

public class TimesheetEntryNotFoundException() : NotFoundException("TimesheetEntry");
