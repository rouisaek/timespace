using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Features.Tenants.Members.Invites.Exceptions;

public class UserAlreadyExistsException() : BadRequestException("User already exists.", "user-already-exists");
