using System.ComponentModel.DataAnnotations;
using System.Web;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.Extensions.Options;
using MimeKit;
using Timespace.Api.Infrastructure.Email;
using Timespace.Api.Infrastructure.Startup;

namespace Timespace.Api.Features.Users.Handlers;

[Handler]
public static partial class SendPasswordResetEmail
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress]
		public required string Email { get; set; } = null!;
		public required string FirstName { get; set; } = null!;
		public required string PasswordResetToken { get; set; } = null!;
	}

	private static async ValueTask HandleAsync(Command command, EmailService emailService, IOptionsSnapshot<SiteSettings> siteSettings, CancellationToken token)
	{
		var emailTemplateProps = new
		{
			UserName = command.FirstName,
			ResetPasswordLink =
				$"{siteSettings.Value.FrontendSiteUrl}/accounts/password-reset?token={HttpUtility.UrlEncode(command.PasswordResetToken)}&email={command.Email}"
		};

		var template = EmailTemplates.ResetPasswordTemplate;

		var renderedHtmlBody = await template.RenderAsync(emailTemplateProps);

		template = EmailTemplates.ResetPasswordTemplatePlain;

		var renderedTextBody = await template.RenderAsync(emailTemplateProps);

		using var mimeMessage = new MimeMessage();
		var builder = new BodyBuilder
		{
			HtmlBody = renderedHtmlBody,
			TextBody = renderedTextBody
		};
		mimeMessage.Subject = "Stel je Timespace wachtwoord opnieuw in";
		mimeMessage.Body = builder.ToMessageBody();
		mimeMessage.To.Add(new MailboxAddress(command.FirstName, command.Email));

		await emailService.SendEmail(mimeMessage, token);
	}
}
