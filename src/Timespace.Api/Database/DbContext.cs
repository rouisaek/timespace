using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Features.Tenants.Models;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<ApplicationUser, int>(options)
{
	public DbSet<Tenant> Tenants { get; init; }
}
