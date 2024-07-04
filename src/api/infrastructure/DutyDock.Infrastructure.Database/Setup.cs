using DutyDock.Infrastructure.Database.Cosmos;
using DutyDock.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Database;

public static class Setup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<DatabaseOptions>(DatabaseOptions.Section, configuration);

        var options = configuration.GetSection(DatabaseOptions.Section).Get<DatabaseOptions>();
        options.ThrowIfNull();

        switch (options.Type)
        {
            case DatabaseOptions.DatabaseType.Cosmos:
                services.AddCosmos(configuration);
                break;
            default:
                throw new NotImplementedException($"Database type '{options.Type}' is not supported.");
        }

        return services;
    }
}