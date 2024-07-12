using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DutyDock.Domain.Identity.User.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum Role
{
    User = 0,
    Manager = 1,
    Admin = 2
}