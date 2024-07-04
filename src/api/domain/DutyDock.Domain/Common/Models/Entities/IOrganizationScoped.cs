namespace DutyDock.Domain.Common.Models.Entities;

/// <summary>
/// Marker interface for entities scoped to an organization.
/// </summary>
public interface IOrganizationScoped
{
    public string OrganizationId { get; }
}