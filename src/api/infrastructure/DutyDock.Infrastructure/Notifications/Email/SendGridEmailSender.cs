using DutyDock.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Throw;

namespace DutyDock.Infrastructure.Notifications.Email;

internal class SendGridEmailSender : EmailSender
{
    private readonly ILogger<SendGridEmailSender> _logger;
    private readonly ISendGridClient _client;

    private readonly EmailAddress _systemAddress;

    public SendGridEmailSender(
        ILogger<SendGridEmailSender> logger,
        IOptions<EmailOptions> options,
        IEnvironmentProvider environmentProvider,
        ISendGridClient client) : base(options, environmentProvider)
    {
        logger.ThrowIfNull();
        client.ThrowIfNull();

        _logger = logger;
        _client = client;

        var emailOptions = options.Value;
        emailOptions.SendGrid!.ApiKey.ThrowIfNull().IfEmpty();

        _systemAddress = new EmailAddress(EmailAddress, Name);
    }

    public override async Task<bool> SendSystem(string subject, string textBody,
        CancellationToken cancellationToken = default)
    {
        return await Send(_systemAddress, _systemAddress, subject, null, textBody, cancellationToken);
    }

    public override async Task<bool> SendUser(string userEmailAddress, string userName, string subject,
        string? htmlBody = null,
        string? textBody = null, CancellationToken cancellationToken = default)
    {
        var userAddress = new EmailAddress(userEmailAddress, userName);
        return await Send(_systemAddress, userAddress, subject, htmlBody, textBody, cancellationToken);
    }

    private async Task<bool> Send(EmailAddress from, EmailAddress to, string subject, string? htmlBody = null,
        string? textBody = null, CancellationToken cancellationToken = default)
    {
        var msg = new SendGridMessage
        {
            From = from,
            Subject = subject
        };

        if (!string.IsNullOrEmpty(htmlBody))
        {
            msg.AddContent(MimeType.Html, htmlBody);
        }

        if (!string.IsNullOrEmpty(textBody))
        {
            msg.AddContent(MimeType.Text, textBody);
        }

        msg.AddTo(to);

        if (Options.UseSandbox)
        {
            msg.MailSettings = new MailSettings
            {
                SandboxMode = new SandboxMode
                {
                    Enable = true
                }
            };
        }

        _logger.LogInformation("Sending email to {ToEmailAddress} ({Subject})", to.Email, subject);

        try
        {
            var response = await _client.SendEmailAsync(msg, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Sending email failed: {StatusCode}", response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sending email failed: {Message}", ex.Message);
            return false;
        }

        return true;
    }
}