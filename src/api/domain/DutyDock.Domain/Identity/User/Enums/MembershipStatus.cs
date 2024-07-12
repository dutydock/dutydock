using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DutyDock.Domain.Identity.User.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum MembershipStatus
{
    Uninvited,
    Invited,
    Active
}