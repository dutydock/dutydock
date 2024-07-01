using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DutyDock.Application;

public static class Setup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(AssemblyReference).Assembly);
        
        return services;
    }
}