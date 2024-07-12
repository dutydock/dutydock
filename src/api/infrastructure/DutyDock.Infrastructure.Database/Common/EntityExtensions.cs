using DutyDock.Domain.Common.Models.Entities;
using Throw;

namespace DutyDock.Infrastructure.Database.Common;

public static class EntityExtensions
{
    public static string GetTypeName(this Type type)
    {
        if (!typeof(IAggregateRoot).IsAssignableFrom(type))
        {
            throw new ArgumentException(
                "Type names should only be applied to aggregate roots", nameof(type));
        }

        var attributes = Attribute.GetCustomAttributes(type);

        var typeNameAttribute = attributes.FirstOrDefault(attribute => attribute is TypeNameAttribute);
        typeNameAttribute.ThrowIfNull();

        return ((TypeNameAttribute)typeNameAttribute).GetName();
    }
    
    public static string GetOwningTypeId<TEntity>(this TEntity entity) where TEntity : Entity
    {
        if (entity is not IAggregateRoot)
        {
            throw new ArgumentException(
                "Owning type id's should only be requested for aggregate roots");
        }

        if (entity is not IOrganizationScoped)
        {
            throw new ArgumentException(
                "Owning type id's should only be requested for organization scoped entities");
        }

        if (entity is not IOwnedEntity)
        {
            throw new ArgumentException(
                "Owning type id's should only be requested for owned entities");
        }

        var properties = entity.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType != typeof(string))
            {
                continue;
            }

            var attributes = property.GetCustomAttributes(true);

            foreach (var attribute in attributes)
            {
                if (attribute is not OwningEntityAttribute)
                {
                    continue;
                }

                var value = property.GetValue(entity);

                if (value == null)
                {
                    throw new ArgumentException("Owning entity id has no value");
                }

                return (string)value;
            }
        }

        throw new AggregateException("No valid owning entity id could be found");
    }
}