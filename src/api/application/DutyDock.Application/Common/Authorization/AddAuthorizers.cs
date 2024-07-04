using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DutyDock.Application.Common.Authorization;

public static class AddAuthorizers
{
    public static void AddAuthorizersFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var authorizerType = typeof(IAuthorizer<>);
        assembly.GetTypesAssignableTo(authorizerType).ForEach((type) =>
        {
            foreach (var implementedInterface in type.ImplementedInterfaces)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Scoped:
                        services.AddScoped(implementedInterface, type);
                        break;
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(implementedInterface, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(implementedInterface, type);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                }
            }
        });
    }

    private static List<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
    {
        return assembly.DefinedTypes.Where(typeInfo => typeInfo is { IsClass: true, IsAbstract: false }
                                                       && typeInfo != compareType
                                                       && typeInfo.GetInterfaces()
                                                           .Any(i => i.IsGenericType
                                                                     && i.GetGenericTypeDefinition() ==
                                                                     compareType)).ToList();
    }
}