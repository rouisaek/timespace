using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Timespace.Api.Infrastructure.Email;

[ConfigureOptions]
public sealed class EmailServiceOptions
{
	public required string Host { get; set; }
	public required int Port { get; set; }
	public required string Password { get; set; }
	public required string Username { get; set; }
	public required string FromEmailAddress { get; set; }
	public required IReadOnlyList<string> AdminEmailAddresses { get; set; }
}

[RegisterScoped]
public sealed class EmailService(IOptionsSnapshot<EmailServiceOptions> options)
{
	private readonly EmailServiceOptions _options = options.Value;

	public async Task SendAdminEmail(string subject, string body, bool isHtml, CancellationToken cancellationToken = default)
	{
		using var message = new MimeMessage();
		message.Subject = subject;
		message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

		await SendAdminEmail(message, cancellationToken);
	}

	public Task SendAdminEmail(MimeMessage message, CancellationToken cancellationToken = default)
	{
		message.To.Clear();
		foreach (var email in _options.AdminEmailAddresses)
			message.To.Add(MailboxAddress.Parse(email));

		return SendEmail(message, cancellationToken);
	}

	public async Task SendEmail(MimeMessage message, CancellationToken cancellationToken = default)
	{
		message.From.Clear();
		message.From.Add(MailboxAddress.Parse(_options.FromEmailAddress));

		using var client = new SmtpClient();
		await client.ConnectAsync(_options.Host, _options.Port, cancellationToken: cancellationToken);

		await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
		_ = await client.SendAsync(message, cancellationToken);

		await client.DisconnectAsync(quit: true, cancellationToken: cancellationToken);
	}
}
