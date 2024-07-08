using Destructurama.Attributed;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Identity;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Users.Endpoints;

[Handler]
[MapPost("/api/login")]
public static partial class LoginEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		public required string Email { get; set; }
		[LogMasked]
		public required string Password { get; set; }
		public required Instant Date { get; set; }
	}

	private static async ValueTask<int> HandleAsync(Command command, AppDbContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, CancellationToken token)
	{
		var user = await userManager.FindByEmailAsync(command.Email);

		if (user == null)
			throw new NotFoundException("User");

		var signInResult = await signInManager.PasswordSignInAsync(user, command.Password, false, false);

		return default;
	}
}
