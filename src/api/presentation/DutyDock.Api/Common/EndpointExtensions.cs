using System.Reflection;
using DutyDock.Api.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Throw;

namespace DutyDock.Api.Common;

internal static class EndpointExtensions
{
    internal static void AddEndpoints(this IServiceCollection services, params Assembly[] applicationParts)
    {
        services.ThrowIfNull();

        var mvcBuilder = services.AddControllers()
            .ConfigureApiBehaviorOptions(Set)
            .AddNewtonsoftJson(Set);

        foreach (var applicationPart in applicationParts)
        {
            mvcBuilder.AddApplicationPart(applicationPart);
        }
    }

    private static void Set(ApiBehaviorOptions options)
    {
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
    }

    private static void Set(MvcNewtonsoftJsonOptions options)
    {
        var jsonSettings = JsonSettings.Get();
        
        options.SerializerSettings.ContractResolver = jsonSettings.ContractResolver;
        options.SerializerSettings.Culture = JsonSettings.DefaultCulture;
        options.SerializerSettings.DateFormatString = JsonSettings.Iso8601DateTimeFormat;
        options.SerializerSettings.ReferenceLoopHandling = jsonSettings.ReferenceLoopHandling;
        options.SerializerSettings.NullValueHandling = jsonSettings.NullValueHandling;
        options.SerializerSettings.Converters = jsonSettings.Converters;
    }
}