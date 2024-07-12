using DutyDock.Infrastructure.Database.Common;

namespace DutyDock.Infrastructure.Database.Cosmos.Repositories.Common;

public static class PartitionHelpers
{
    public static string GetApplicationScopedPartitionKey(string entityId)
    {
        return entityId;
    }
    
    public static string GetOrganizationScopedPartitionKey(Type type, string organizationId)
    {
        return $"{type.GetTypeName()}-{organizationId}";
    }
    
    public static string GetResourceScopedPartitionKey(Type type, string organizationId, string owningEntityId)
    {
        return $"{type.GetTypeName()}-{organizationId}-{owningEntityId}";
    }
}