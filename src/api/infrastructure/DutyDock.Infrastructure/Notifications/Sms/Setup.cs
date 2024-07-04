using DutyDock.Application.Common.Interfaces.Notifications;
using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Notifications.Sms;

public static class Setup
{
    public static IServiceCollection AddSms(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        const string sectionName = $"{NotificationOptions.Section}:{SmsOptions.Section}";
        services.ConfigureAndValidate<SmsOptions>(sectionName, configuration);

        var options = configuration.GetSection(sectionName).Get<SmsOptions>();
        options.ThrowIfNull();

        switch (options.Sender)
        {
            case SmsOptions.SmsSender.Twilio:
                services.AddSingleton<ISmsSender, TwilioSmsSender>();
                break;
            case SmsOptions.SmsSender.Console:
                services.AddSingleton<ISmsSender, ConsoleSmsSender>();
                break;
            default:
                throw new NotImplementedException($"Sms sender '{options.Sender}' is not supported.");
        }

        return services;
    }
}