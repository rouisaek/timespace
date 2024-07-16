using System.Diagnostics.CodeAnalysis;

namespace Timespace.Api.Infrastructure.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public class TimespaceException(int status, string message, string type) : Exception(message)
{
	public int StatusCode { get; set; } = status;
	public string ErrorType { get; set; } = type;
}
