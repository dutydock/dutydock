using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Security.Cors;

public static class Setup
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        const string sectionName = $"{SecurityOptions.Section}:{CorsOptions.Section}";
        services.ConfigureAndValidate<CorsOptions>(sectionName, configuration);

        var options = configuration.GetSection(sectionName).Get<CorsOptions>();
        options.ThrowIfNull();
        
        services.AddCors(corsOptions =>
        {
            corsOptions.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(options.AllowedOrigins?.ToArray() ?? Array.Empty<string>())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        return services;
    }
}