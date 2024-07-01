using System.Reflection;
using Microsoft.OpenApi.Models;
using Throw;

namespace DutyDock.Api.Common;

internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.ThrowIfNull();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(GetDoc(), GetInfo());
            options.EnableAnnotations();
            options.IncludeXmlComments(GetXmlPath(typeof(Startup).Assembly));
            options.CustomSchemaIds(selector => selector.FullName);
        });

        services.AddSwaggerGenNewtonsoftSupport();
        
        return services;
    }

    internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.ThrowIfNull();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(GetJsonPath(), GetDefinition());
            options.DefaultModelsExpandDepth(-1); // No schemas section
        });

        return app;
    }

    private static string GetDoc(string version = "v1")
    {
        return version;
    }

    private static string GetDefinition(string appName = "DutyDock", string version = "v1")
    {
        return $"{appName} {version}";
    }

    private static string GetJsonPath(string version = "v1")
    {
        return $"/swagger/{version}/swagger.json";
    }

    private static string GetXmlPath(Assembly assembly)
    {
        var assemblyName = assembly.GetName().Name;
        return Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");
    }

    private static OpenApiInfo GetInfo(string appName = "DutyDock", string version = "v1")
    {
        return new OpenApiInfo
        {
            Title = appName,
            Version = version,
            Description = $"The {appName} API specification.",
            Contact = new OpenApiContact { Name = "DutyDock", Email = "hello@dutydock.com" }
        };
    }
}