using Timespace.Api.Database.EntityAttributes;

namespace Timespace.Api.Features.Tenants.Models;

public class Tenant : IEntity
{
	public int Id { get; set; }
}
