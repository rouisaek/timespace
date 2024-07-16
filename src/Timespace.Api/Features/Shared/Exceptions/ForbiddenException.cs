using System.Diagnostics.CodeAnalysis;
using Timespace.Api.Infrastructure.Exceptions;

namespace Timespace.Api.Features.Shared.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public class ForbiddenException() : TimespaceException(StatusCodes.Status403Forbidden, $"Missing permissions", "timespace:errors:missing-permissions");
