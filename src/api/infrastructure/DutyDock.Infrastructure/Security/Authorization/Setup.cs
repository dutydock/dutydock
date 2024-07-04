using DutyDock.Application.Common.Interfaces.Security;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Security.Authorization;

public static class Setup
{
    public static IServiceCollection AddAuthorizationConfig(this IServiceCollection services)
    {
        services.ThrowIfNull();
        
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddScoped<ICachedUserProvider, CachedUserProvider>();
        
        return services;
    }
}