using DutyDock.Api.Common;

namespace DutyDock.Api;

public static class Setup
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSwaggerDocumentation();
        
        var webAssembly = typeof(Web.AssemblyReference).Assembly;
        services.AddEndpoints(webAssembly);
        
        return services;
    }
}