using NodaTime;

namespace Timespace.Api.Database.Common;

public interface ISoftDeletable
{
	public Instant? DeletedAt { get; set; }
}
