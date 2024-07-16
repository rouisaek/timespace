using Destructurama.Attributed;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Identity;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Validations;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/register")]
public static partial class RegisterEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public required string TenantName { get; set; }

		[EmailAddress]
		public required string Email { get; init; }
		[LogMasked]
		public required string Password { get; init; }
	}

	public record Response
	{
		public bool Success { get; set; }
	}

	private static async ValueTask<Response> HandleAsync(Command command, UserManager<ApplicationUser> userManager, AppDbContext dbContext, CancellationToken token)
	{
		var tenant = dbContext.Tenants.Add(new() { DisplayName = command.TenantName, });
		_ = await dbContext.SaveChangesAsync(token);

		var result = await userManager.CreateAsync(new ApplicationUser() { Email = command.Email, UserName = command.Email, TenantId = tenant.Entity.Id }, command.Password);

		if (result.Succeeded)
			return new()
			{
				Success = true
			};

		return new() { Success = false, };
	}
}
