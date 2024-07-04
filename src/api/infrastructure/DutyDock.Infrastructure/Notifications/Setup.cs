using DutyDock.Infrastructure.Notifications.Email;
using DutyDock.Infrastructure.Notifications.Sms;
using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DutyDock.Infrastructure.Notifications;

public static class Setup
{
    public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<NotificationOptions>(NotificationOptions.Section, configuration);

        services
            .AddEmail(configuration)
            .AddSms(configuration);

        return services;
    }
}