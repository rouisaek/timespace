using Timespace.Api.Features.Shared.Exceptions;

namespace Timespace.Api.Features.Users.EmailConfirmation.Exceptions;

public class EmailConfirmationFailedException()
	: BadRequestException("Email confirmation failed.", "email-confirmation-failed");
