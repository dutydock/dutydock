using DutyDock.Application.Common.Interfaces.Notifications;
using DutyDock.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using Throw;

namespace DutyDock.Infrastructure.Notifications.Email;

internal abstract class EmailSender : IEmailSender
{
    protected readonly EmailOptions Options;
    protected readonly string Name;
    protected readonly string EmailAddress;

    protected EmailSender(IOptions<EmailOptions> options, IEnvironmentProvider environmentProvider)
    {
        options.ThrowIfNull();
        environmentProvider.ThrowIfNull();

        Options = options.Value;

        Options.Name.ThrowIfNull();
        Options.EmailAddress.ThrowIfNull();

        Name = environmentProvider.IsProduction
            ? Options.Name
            : $"{Options.Name} ({environmentProvider.Name})";
        EmailAddress = Options.EmailAddress;
    }

    public abstract Task<bool> SendSystem(string subject, string textBody,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> SendUser(string toEmailAddress, string toName, string subject, string? htmlBody = null,
        string? textBody = null,
        CancellationToken cancellationToken = default);
}