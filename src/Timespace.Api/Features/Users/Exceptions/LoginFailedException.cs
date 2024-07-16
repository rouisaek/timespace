using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Features.Users.Exceptions;

public class LoginFailedException() : BadRequestException("Email or password incorrect", "login-failed");
