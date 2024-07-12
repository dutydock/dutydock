using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DutyDock.Domain.Identity.Organization.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum OrganizationStatus
{
    Created,
    Initialized,
    Error
}