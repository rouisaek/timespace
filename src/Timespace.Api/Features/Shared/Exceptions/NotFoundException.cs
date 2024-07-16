using System.Diagnostics.CodeAnalysis;
using Timespace.Api.Infrastructure.Exceptions;

namespace Timespace.Api.Features.Shared.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public class NotFoundException(string entityName) : TimespaceException(StatusCodes.Status404NotFound, $"{entityName} could not be found.", "not-found");
