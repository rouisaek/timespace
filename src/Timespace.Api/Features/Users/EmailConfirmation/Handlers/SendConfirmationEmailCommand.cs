using System.ComponentModel.DataAnnotations;
using System.Web;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using NodaTime;
using Timespace.Api.Features.Users.Models;
using Timespace.Api.Infrastructure;
using Timespace.Api.Infrastructure.Email;
using Timespace.Api.Infrastructure.Startup;

namespace Timespace.Api.Features.Users.EmailConfirmation.Handlers;

[Handler]
public static partial class SendConfirmationEmailCommand
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress]
		public required string Email { get; set; } = null!;
		public required string FirstName { get; set; } = null!;
		public required ApplicationUser User { get; set; } = null!;
	}
	private static async ValueTask HandleAsync(Command command, EmailService emailService, UserManager<ApplicationUser> userManager, IClock clock, IOptionsSnapshot<SiteSettings> siteSettings, CancellationToken token)
	{
		var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(command.User);

		var emailTemplateProps = new
		{
			UserName = command.FirstName,
			ConfirmEmailLink =
				$"{siteSettings.Value.FrontendSiteUrl}/accounts/email-confirmation?token={HttpUtility.UrlEncode(emailConfirmationToken)}&email={command.Email}"
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
		mimeMessage.To.Add(new MailboxAddress(command.FirstName, command.Email));

		await emailService.SendEmail(mimeMessage, token);

		var updatedUser = command.User;
		updatedUser.LastEmailConfirmationSent = clock.GetCurrentInstant();

		_ = await userManager.UpdateAsync(updatedUser);
	}
}
