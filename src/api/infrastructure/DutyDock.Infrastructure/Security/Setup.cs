using DutyDock.Infrastructure.Security.Authentication;
using DutyDock.Infrastructure.Security.Authorization;
using DutyDock.Infrastructure.Security.Cors;
using DutyDock.Infrastructure.Security.DataProtection;
using DutyDock.Infrastructure.Security.Password;
using DutyDock.Infrastructure.Security.Tokens;
using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DutyDock.Infrastructure.Security;

public static class Setup
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<SecurityOptions>(SecurityOptions.Section, configuration);

        services
            .AddCorsPolicy(configuration)
            .AddDataProtectionConfig(configuration)
            .AddTokens(configuration)
            .AddPasswordProtection()
            .AddAuthenticationConfig(configuration)
            .AddAuthorizationConfig();
        
        return services;
    }
}