using DutyDock.Infrastructure.Database;
using DutyDock.Infrastructure.Notifications;
using DutyDock.Infrastructure.Security;
using DutyDock.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure;

public static class Setup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        services
            .AddOptions()
            .AddServices()
            .AddNotifications(configuration)
            .AddSecurity(configuration)
            .AddDatabase(configuration);
        
        return services;
    }
}