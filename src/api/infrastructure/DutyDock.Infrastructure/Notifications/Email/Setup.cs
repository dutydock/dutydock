using DutyDock.Application.Common.Interfaces.Notifications;
using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Notifications.Email;

public static class Setup
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        const string sectionName = $"{NotificationOptions.Section}:{EmailOptions.Section}";
        services.ConfigureAndValidate<EmailOptions>(sectionName, configuration);

        var options = configuration.GetSection(sectionName).Get<EmailOptions>();
        options.ThrowIfNull();

        switch (options.Sender)
        {
            case EmailOptions.EmailSender.SendGrid:
                options.SendGrid.ThrowIfNull();
                services.AddSendGrid(clientOptions => { clientOptions.ApiKey = options.SendGrid.ApiKey; });
                services.AddSingleton<IEmailSender, SendGridEmailSender>();
                break;
            case EmailOptions.EmailSender.Smtp:
                options.Smtp.ThrowIfNull();
                services.AddSingleton<IEmailSender, SmtpEmailSender>();
                break;
            case EmailOptions.EmailSender.Console:
                services.AddSingleton<IEmailSender, ConsoleEmailSender>();
                break;
            default:
                throw new NotImplementedException($"Email sender '{options.Sender}' is not supported.");
        }

        return services;
    }
}