using System.Text.RegularExpressions;
using Immediate.Validations.Shared;

namespace Timespace.Api.Features.Shared.Validations;

public sealed partial class EmailAddressAttribute : ValidatorAttribute
{
	public static bool ValidateProperty(string target)
		=> EmailAddressRegex().IsMatch(target);

	public const string DefaultMessage = "'{PropertyName}' is not a valid email address.";

	[GeneratedRegex("^[^@]+@[^@]+$")]
	private static partial Regex EmailAddressRegex();
}
