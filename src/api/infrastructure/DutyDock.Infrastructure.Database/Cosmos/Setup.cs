using DutyDock.Infrastructure.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Throw;

namespace DutyDock.Infrastructure.Database.Cosmos;

internal static class Setup
{
    private const int DefaultMaxRetryAttemptsOnRateLimitedRequests = 20;
    private const int DefaultMaxRetryWaitTimeOnRateLimitedRequestsInSec = 30;

    private static JsonSerializerSettings SerializerSettings
    {
        get
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var converters = jsonSettings.Converters;
            converters.Add(new StringEnumConverter());

            return jsonSettings;
        }
    }

    public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        services
            .AddDatabase(configuration)
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        const string sectionName = $"{DatabaseOptions.Section}:{CosmosOptions.Section}";
        services.ConfigureAndValidate<CosmosOptions>(sectionName, configuration);

        var options = configuration.GetSection(sectionName).Get<CosmosOptions>();
        options.ThrowIfNull();

        var serializer = new JsonCosmosSerializer(SerializerSettings);

        var clientOptions = GetClientOptions(serializer, options);
        var client = new CosmosClient(options.Endpoint, options.AccessKey, clientOptions);

        CosmosInitializer.Execute(options, client).Wait();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }

    private static CosmosClientOptions GetClientOptions(CosmosSerializer serializer, CosmosOptions options,
        bool isBulkClient = false)
    {
        var clientOptions = new CosmosClientOptions
        {
            Serializer = serializer,
            ConnectionMode = options.IsEmulator ? ConnectionMode.Gateway : ConnectionMode.Direct,
            LimitToEndpoint = options.IsEmulator,
            AllowBulkExecution = isBulkClient,
            MaxRetryAttemptsOnRateLimitedRequests = DefaultMaxRetryAttemptsOnRateLimitedRequests,
            MaxRetryWaitTimeOnRateLimitedRequests =
                TimeSpan.FromSeconds(DefaultMaxRetryWaitTimeOnRateLimitedRequestsInSec)
        };

        DisableServerCertificateCheckIfNeeded(options.IsEmulator, clientOptions);

        return clientOptions;
    }

    private static void DisableServerCertificateCheckIfNeeded(bool isEmulator, CosmosClientOptions clientOptions)
    {
        if (!isEmulator)
        {
            return;
        }

        clientOptions.HttpClientFactory = () => new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
    }
}