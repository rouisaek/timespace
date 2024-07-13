using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database.Common;
using Timespace.Api.Database.Extensions;
using Timespace.Api.Features.Tenants.Models;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<ApplicationUser, int>(options)
{
	public DbSet<Tenant> Tenants { get; init; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		_ = builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

		// Expression<Func<ITenanted, bool>> tenantExpression =
		// 	entity => entity.TenantId == _usageContext.CurrentTenantId;
		Expression<Func<ISoftDeletable, bool>> softDeleteExpression = entity => entity.DeletedAt == null;

		foreach (var entityType in builder.Model.GetEntityTypes())
		{
			// if (entityType.ClrType.IsAssignableTo(typeof(ITenanted)))
			// {
			// 	modelBuilder.Entity(entityType.ClrType).AppendQueryFilter(tenantExpression);
			// }

			if (entityType.ClrType.IsAssignableTo(typeof(ISoftDeletable)))
			{
				builder.Entity(entityType.ClrType).AppendQueryFilter(softDeleteExpression);
			}
		}
	}
}
