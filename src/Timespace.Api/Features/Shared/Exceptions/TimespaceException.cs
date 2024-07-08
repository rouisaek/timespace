using System.Diagnostics.CodeAnalysis;

namespace Timespace.Api.Features.Shared.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public class TimespaceException(int status, string message) : Exception(message)
{
	public int StatusCode { get; set; } = status;
}
