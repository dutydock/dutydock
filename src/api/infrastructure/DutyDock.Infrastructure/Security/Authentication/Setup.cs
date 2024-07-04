using DutyDock.Infrastructure.Security.Authentication.Cookie;
using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Security.Authentication;

public static class Setup
{
    public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();
        
        const string sectionName = $"{SecurityOptions.Section}:{AuthenticationOptions.Section}";
        services.ConfigureAndValidate<AuthenticationOptions>(sectionName, configuration);

        services
            .AddCookieAuthentication(configuration);

        return services;
    }
}