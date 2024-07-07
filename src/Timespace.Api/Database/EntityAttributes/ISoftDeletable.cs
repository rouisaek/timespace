using NodaTime;

namespace Timespace.Api.Database.EntityAttributes;

public class ISoftDeletable
{
	public bool Deleted { get; set; }
	public Instant DeletedAt { get; set; }
}
