using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NodaTime;
using Timespace.Api.Database.Common;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Database.Interceptors;

[RegisterScoped(typeof(SaveChangesInterceptor))]
public class SaveChangesInterceptor(IClock clock, IUsageContext usageContext) : ISaveChangesInterceptor
{
	public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
		CancellationToken cancellationToken = new CancellationToken())
	{
		if (eventData.Context is null)
			return new ValueTask<InterceptionResult<int>>(result);

		var timestampedEntries = eventData.Context.ChangeTracker
			.Entries()
			.Where(e => e is { Entity: ITimestamped, State: EntityState.Added or EntityState.Modified });

		foreach (var entityEntry in timestampedEntries)
		{
			var currentInstant = clock.GetCurrentInstant();
			((ITimestamped)entityEntry.Entity).UpdatedAt = currentInstant;

			if (entityEntry.State == EntityState.Added)
			{
				((ITimestamped)entityEntry.Entity).CreatedAt = currentInstant;
			}
		}

		var softdeleteEntries = eventData.Context.ChangeTracker
			.Entries()
			.Where(e => e is { Entity: ISoftDeletable, State: EntityState.Deleted });

		foreach (var entityEntry in softdeleteEntries)
		{
			((ISoftDeletable)entityEntry.Entity).DeletedAt = clock.GetCurrentInstant();
		}

		var tenantedEntries = eventData.Context.ChangeTracker
			.Entries()
			.Where(e => e is { Entity: ITenanted, State: EntityState.Added or EntityState.Modified });

		foreach (var entityEntry in tenantedEntries)
		{
			if (usageContext.TenantId != null)
				((ITenanted)entityEntry.Entity).TenantId = usageContext.TenantId.Value;
		}

		return new ValueTask<InterceptionResult<int>>(result);
	}

	public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		throw new InvalidOperationException();
	}
}
