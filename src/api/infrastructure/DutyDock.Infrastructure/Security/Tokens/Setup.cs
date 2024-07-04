using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Security.Tokens;

public static class Setup
{
    public static IServiceCollection AddTokens(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        const string sectionName = $"{SecurityOptions.Section}:{TokenOptions.Section}";
        services.ConfigureAndValidate<TokenOptions>(sectionName, configuration);

        services.AddTransient<IPasswordResetUserTokenProvider, PasswordResetUserTokenProvider>();
        services.AddTransient<IMemberInvitationUserTokenProvider, MemberInvitationUserTokenProvider>();
        
        return services;
    }
}