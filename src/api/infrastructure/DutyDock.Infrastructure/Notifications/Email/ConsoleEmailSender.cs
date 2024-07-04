using System.Text;
using DutyDock.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Throw;

namespace DutyDock.Infrastructure.Notifications.Email;

internal class ConsoleEmailSender : EmailSender
{
    private const string Separator = "*************************************************************";

    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger, IOptions<EmailOptions> options,
        IEnvironmentProvider environmentProvider) : base(options, environmentProvider)
    {
        logger.ThrowIfNull();
        _logger = logger;
    }

    public override Task<bool> SendSystem(string subject, string textBody,
        CancellationToken cancellationToken = default)
    {
        return Send(EmailAddress, Name, EmailAddress, Name, subject, null, textBody);
    }

    public override Task<bool> SendUser(string userEmailAddress, string userName, string subject,
        string? htmlBody = null,
        string? textBody = null,
        CancellationToken cancellationToken = default)
    {
        return Send(EmailAddress, Name, userEmailAddress, userName, subject, htmlBody, textBody);
    }

    private Task<bool> Send(string fromEmailAddress, string fromName, string toEmailAddress, string toName,
        string subject, string? htmlBody = null,
        string? textBody = null)
    {
        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine(Separator);
        builder.AppendLine($"From:    {fromName} - {fromEmailAddress}");
        builder.AppendLine($"To:      {toName} - {toEmailAddress}");
        builder.AppendLine($"Subject: {subject}");

        if (!string.IsNullOrEmpty(htmlBody))
        {
            builder.AppendLine(Separator);
            builder.AppendLine(htmlBody);
        }

        if (!string.IsNullOrEmpty(textBody))
        {
            builder.AppendLine(Separator);
            builder.AppendLine(textBody);
        }

        builder.AppendLine(Separator);
        builder.AppendLine();

        _logger.LogInformation("{Message}", builder.ToString());

        return Task.FromResult(true);
    }
}