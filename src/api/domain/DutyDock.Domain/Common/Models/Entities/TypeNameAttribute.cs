namespace DutyDock.Domain.Common.Models.Entities;

/// <summary>
/// Aggregate root type name attribute.
/// </summary>
/// <summary>
/// The type name attribute is used to annotate aggregate roots with a
/// constant and stable name for the type. This in turn is used by the persistence
/// layer to query entities of a particular type.
/// The type name value should not change unless a migration strategy is in place.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TypeNameAttribute : Attribute
{
    private readonly string _name;

    public TypeNameAttribute(string name)
    {
        _name = name;
    }

    public string GetName()
    {
        return _name;
    }
}