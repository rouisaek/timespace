using Scriban;
using Timespace.Api.Infrastructure.Email.Templates;

namespace Timespace.Api.Infrastructure.Email;

public static class EmailTemplates
{
	public static Template ConfirmEmailTemplate => ScribanTemplateUtility.GetScribanTemplate("ConfirmEmail");
	public static Template ConfirmEmailTemplatePlain => ScribanTemplateUtility.GetScribanTemplate("ConfirmEmailPlain");
}
