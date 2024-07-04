using DutyDock.Domain.Iam.User.Services;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Security.Password;

public static class Setup
{
    public static IServiceCollection AddPasswordProtection(this IServiceCollection services)
    {
        services.ThrowIfNull();

        services.AddTransient<IUserPasswordHasher, BCryptPasswordHasher>();
        
        return services;
    }
}