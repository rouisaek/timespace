using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timespace.Api.Database.EntityAttributes;

namespace Timespace.Api.Features.Tenants.Models;

public class Tenant : IEntity
{
	public int Id { get; set; }

	[MaxLength(256)]
	public string DisplayName { get; init; } = null!;

	public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
	{
		public void Configure(EntityTypeBuilder<Tenant> builder)
		{
			throw new NotImplementedException();
		}
	}
}

