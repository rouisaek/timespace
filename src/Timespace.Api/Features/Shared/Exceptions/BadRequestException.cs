using System.Diagnostics.CodeAnalysis;

namespace Timespace.Api.Features.Shared.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public class BadRequestException(string message) : TimespaceException(StatusCodes.Status400BadRequest, message);
