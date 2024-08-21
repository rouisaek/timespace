using System.ComponentModel.DataAnnotations;
using System.Web;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;
using Microsoft.Extensions.Options;
using MimeKit;
using Timespace.Api.Infrastructure.Email;
using Timespace.Api.Infrastructure.Startup;

namespace Timespace.Api.Features.Tenants.Members.Invites.Handlers;

[Handler]
public static partial class SendInviteEmailToUser
{
	[Validate]
	public partial record Command : IValidationTarget<Command>
	{
		[EmailAddress]
		public required string Email { get; set; } = null!;
		public required string FirstName { get; set; } = null!;
		public required string InviteToken { get; set; } = null!;
		public required string TenantName { get; set; } = null!;
	}

	private static async ValueTask HandleAsync(Command command, EmailService emailService, IOptionsSnapshot<SiteSettings> siteSettings, CancellationToken token)
	{
		var emailTemplateProps = new
		{
			UserName = command.FirstName,
			command.TenantName,
			InviteLink =
				$"{siteSettings.Value.FrontendSiteUrl}/accounts/invite/{HttpUtility.UrlEncode(command.InviteToken)}"
		};

		var template = EmailTemplates.InviteReceivedTemplate;

		var renderedHtmlBody = await template.RenderAsync(emailTemplateProps);

		template = EmailTemplates.InviteReceivedTemplatePlain;

		var renderedTextBody = await template.RenderAsync(emailTemplateProps);

		using var mimeMessage = new MimeMessage();
		var builder = new BodyBuilder
		{
			HtmlBody = renderedHtmlBody,
			TextBody = renderedTextBody
		};
		mimeMessage.Body = builder.ToMessageBody();
		mimeMessage.Subject = "Uitnodiging voor Timespace van " + command.TenantName;
		mimeMessage.To.Add(new MailboxAddress(command.FirstName, command.Email));

		await emailService.SendEmail(mimeMessage, token);
	}
}
