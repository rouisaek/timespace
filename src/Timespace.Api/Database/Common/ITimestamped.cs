using NodaTime;

namespace Timespace.Api.Database.Common;

public interface ITimestamped
{
	public Instant CreatedAt { get; set; }
	public Instant? UpdatedAt { get; set; }
}
