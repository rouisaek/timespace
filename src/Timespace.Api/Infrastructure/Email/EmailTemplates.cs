using Scriban;
using Timespace.Api.Infrastructure.Email.Templates;

namespace Timespace.Api.Infrastructure.Email;

public static class EmailTemplates
{
	public static Template ConfirmEmailTemplate => ScribanTemplateUtility.GetScribanTemplate("ConfirmEmail");
	public static Template ConfirmEmailTemplatePlain => ScribanTemplateUtility.GetScribanTemplate("ConfirmEmailPlain");
	public static Template InviteReceivedTemplate => ScribanTemplateUtility.GetScribanTemplate("InviteReceived");
	public static Template InviteReceivedTemplatePlain => ScribanTemplateUtility.GetScribanTemplate("InviteReceivedPlain");
	public static Template ResetPasswordTemplate => ScribanTemplateUtility.GetScribanTemplate("ResetPassword");
	public static Template ResetPasswordTemplatePlain => ScribanTemplateUtility.GetScribanTemplate("ResetPasswordPlain");
}
