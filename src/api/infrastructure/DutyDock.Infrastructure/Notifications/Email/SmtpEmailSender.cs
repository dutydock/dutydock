using DutyDock.Application.Common.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Throw;

namespace DutyDock.Infrastructure.Notifications.Email;

/**
 * To test with MailHog (https://hub.docker.com/r/mailhog/mailhog)
 * $ docker run -it --rm -p 1025:1025 -p 8025:8025 mailhog/mailhog:latest
 *
 * Server: 127.0.0.1
 * Username: null
 * Password: null
 * Port: 1025
 */
internal class SmtpEmailSender : EmailSender
{
    private readonly ILogger<SmtpEmailSender> _logger;

    private readonly MailboxAddress _systemAddress;

    public SmtpEmailSender(
        ILogger<SmtpEmailSender> logger,
        IOptions<EmailOptions> options,
        IEnvironmentProvider environmentProvider) : base(options, environmentProvider)
    {
        logger.ThrowIfNull();
        _logger = logger;

        var emailOptions = options.Value;
        emailOptions.Smtp!.Server.ThrowIfNull().IfEmpty();

        _systemAddress = new MailboxAddress(Name, EmailAddress);
    }

    public override async Task<bool> SendSystem(string subject, string textBody,
        CancellationToken cancellationToken = default)
    {
        return await Send(_systemAddress, _systemAddress, subject, null, textBody, cancellationToken);
    }

    public override async Task<bool> SendUser(string userEmailAddress, string userName, string subject,
        string? htmlBody = null,
        string? textBody = null,
        CancellationToken cancellationToken = default)
    {
        var userAddress = new MailboxAddress(userName, userEmailAddress);
        return await Send(_systemAddress, userAddress, subject, htmlBody, textBody, cancellationToken);
    }

    private async Task<bool> Send(InternetAddress from, InternetAddress to, string subject, string? htmlBody = null,
        string? textBody = null, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(from);
        message.To.Add(to);

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = textBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(Options.Smtp!.Server, (int)Options.Smtp!.Port,
                cancellationToken: cancellationToken);

            if (!string.IsNullOrEmpty(Options.Smtp.Username))
            {
                await client.AuthenticateAsync(Options.Smtp.Username, Options.Smtp.Password,
                    cancellationToken: cancellationToken);
            }

            await client.SendAsync(message, cancellationToken: cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sending email failed: {Message}", ex.Message);
            return false;
        }

        return true;
    }
}