using MimeKit;

namespace Timespace.Api.Infrastructure.Email;

public record SendEmailProps(string UserName, string ConfirmEmailLink);

public static class EmailServiceExtensions
{
	public static async Task SendConfirmEmail(this EmailService emailService, SendEmailProps props, string to)
	{
		var template = EmailTemplates.ConfirmEmailTemplate;

		var renderedHtmlBody = await template.RenderAsync(props);

		template = EmailTemplates.ConfirmEmailTemplatePlain;

		var renderedTextBody = await template.RenderAsync(props);

		using var mimeMessage = new MimeMessage();
		var builder = new BodyBuilder
		{
			HtmlBody = renderedHtmlBody,
			TextBody = renderedTextBody
		};
		mimeMessage.Body = builder.ToMessageBody();
		mimeMessage.To.Add(new MailboxAddress(props.UserName, to));

		await emailService.SendEmail(mimeMessage);
	}
}
