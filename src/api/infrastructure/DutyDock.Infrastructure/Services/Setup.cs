using DutyDock.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Services;

public static class Setup
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.ThrowIfNull();
        
        services.AddSingleton<IEnvironmentProvider, EnvironmentProvider>();
        services.AddSingleton<IAppInfoProvider, AppInfoProvider>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}