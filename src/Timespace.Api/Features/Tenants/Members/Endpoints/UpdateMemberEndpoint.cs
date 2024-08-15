using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timespace.Api.Database;
using Timespace.Api.Features.Shared.Exceptions;
using Timespace.Api.Features.Users.Models;

namespace Timespace.Api.Features.Tenants.Members.Endpoints;

[GeneratePermissionPolicy]
public static class UpdateMemberEndpointPolicy
{
	public const string PolicyName = "timespace:tenant:members:update";
}

[Handler]
[MapPut("/api/tenant/members/{userId}")]
[Authorize(UpdateMemberEndpointPolicy.PolicyName)]
public static partial class UpdateMemberEndpoint
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[FromRoute(Name = "userId")]
		public int UserId { get; set; }
		[FromBody]
		public CommandBody Body { get; set; } = null!;
	}

	[Validate]
	public partial record CommandBody : IValidationTarget<CommandBody>
	{
		public string FirstName { get; set; } = null!;
		public string? MiddleName { get; set; }
		public string? LastName { get; set; }
		public string? EmployeeCode { get; set; }
	}

	public record Response;

	private static async ValueTask<Response> HandleAsync([AsParameters] Command command, AppDbContext db, UserManager<ApplicationUser> userManager, CancellationToken token)
	{
		var tenantUser = await db.TenantUsers.Include(x => x.User)
			.FirstOrDefaultAsync(x => x.UserId == command.UserId, token) ?? throw new NotFoundException("User");

		tenantUser.User.FirstName = command.Body.FirstName;
		tenantUser.User.MiddleName = command.Body.MiddleName;
		tenantUser.User.LastName = command.Body.LastName;
		tenantUser.EmployeeCode = command.Body.EmployeeCode;

		_ = await db.SaveChangesAsync(token);

		return new Response();
	}
}

