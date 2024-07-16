using System.Diagnostics.CodeAnalysis;
using Timespace.Api.Infrastructure.Exceptions;

namespace Timespace.Api.Features.Shared.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public class BadRequestException(string message, string type = "bad-request") : TimespaceException(StatusCodes.Status400BadRequest, message, type);
