using System.ComponentModel.DataAnnotations;
using System.Web;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using NodaTime;
using Timespace.Api.Database;
using Timespace.Api.Infrastructure.Email;
using Timespace.Api.Infrastructure.Startup;
using Timespace.Api.Infrastructure.UsageContext;

namespace Timespace.Api.Features.Users.EmailConfirmation.Handlers;

[Handler]
public static partial class SendConfirmationEmail
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress]
		public required string Email { get; set; } = null!;
		public required string FirstName { get; set; } = null!;
		public required string EmailVerificationToken { get; set; } = null!;
		public required int UserId { get; set; }
	}

	private static async ValueTask HandleAsync(Command command, EmailService emailService, AppDbContext db, IClock clock, IUsageContext usageContext, IOptionsSnapshot<SiteSettings> siteSettings, CancellationToken token)
	{
		var emailTemplateProps = new
		{
			UserName = command.FirstName,
			ConfirmEmailLink =
				$"{siteSettings.Value.FrontendSiteUrl}/accounts/email-confirmation?token={HttpUtility.UrlEncode(command.EmailVerificationToken)}&email={command.Email}"
		};

		var template = EmailTemplates.ConfirmEmailTemplate;

		var renderedHtmlBody = await template.RenderAsync(emailTemplateProps);

		template = EmailTemplates.ConfirmEmailTemplatePlain;

		var renderedTextBody = await template.RenderAsync(emailTemplateProps);

		using var mimeMessage = new MimeMessage();
		var builder = new BodyBuilder
		{
			HtmlBody = renderedHtmlBody,
			TextBody = renderedTextBody
		};
		mimeMessage.Body = builder.ToMessageBody();
		mimeMessage.Subject = "Verifieer je e-mailadres voor Timespace";
		mimeMessage.To.Add(new MailboxAddress(command.FirstName, command.Email));

		await emailService.SendEmail(mimeMessage, token);

		var currentInstant = clock.GetCurrentInstant();
		_ = await db.Users
			.Where(x => x.Id == command.UserId)
			.ExecuteUpdateAsync(setters =>
				setters.SetProperty(x => x.LastEmailConfirmationSent, currentInstant), token);
	}
}
