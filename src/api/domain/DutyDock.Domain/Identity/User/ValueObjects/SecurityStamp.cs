using DutyDock.Domain.Common.Models.ValueObjects;
using DutyDock.Domain.Common.Services;
using Newtonsoft.Json;

namespace DutyDock.Domain.Identity.User.ValueObjects;

public class SecurityStamp : ValueObject
{
    [JsonProperty] public string Value { get; private set; } = null!;

    private SecurityStamp()
    {
    }

    private SecurityStamp(string value)
    {
        Value = value;
    }

    internal static SecurityStamp Create()
    {
        return new SecurityStamp(Generate());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static string Generate()
    {
        return IdentityProvider.New();
    }
}