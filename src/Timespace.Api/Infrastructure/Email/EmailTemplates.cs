using Scriban;

namespace Timespace.Api.Infrastructure.Email;

public static class EmailTemplates
{
	public static Template ConfirmEmailTemplate => ScribanTemplateUtility.GetScribanTemplate("ConfirmEmail");
}
