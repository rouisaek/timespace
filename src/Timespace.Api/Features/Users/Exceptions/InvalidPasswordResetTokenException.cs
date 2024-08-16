using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Features.Users.Exceptions;

public class InvalidPasswordResetTokenException()
	: BadRequestException("Invalid password reset token.", "invalid-password-reset-token");
