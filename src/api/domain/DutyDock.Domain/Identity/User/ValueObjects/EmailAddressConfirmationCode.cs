using System.Security.Cryptography;
using DutyDock.Domain.Common.Models.ValueObjects;
using Newtonsoft.Json;

namespace DutyDock.Domain.Identity.User.ValueObjects;

public sealed class EmailAddressConfirmationCode : ValueObject
{
    private static readonly TimeSpan ValidityInterval = TimeSpan.FromMinutes(5);

    [JsonProperty] public string Code { get; private set; } = null!;

    [JsonProperty] public DateTime CreatedAt { get; private set; }

    private EmailAddressConfirmationCode()
    {
    }

    private EmailAddressConfirmationCode(string code, DateTime createdAt)
    {
        Code = code;
        CreatedAt = createdAt;
    }

    internal static EmailAddressConfirmationCode Create()
    {
        var code = GenerateCode();
        var createdAt = DateTime.UtcNow;

        return new EmailAddressConfirmationCode(code, createdAt);
    }

    public bool IsValid(string? code)
    {
        if (code == null)
        {
            return false;
        }

        return !IsExpired() && code == Code;
    }

    public bool IsExpired()
    {
        return CreatedAt + ValidityInterval < DateTime.UtcNow;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
        yield return CreatedAt;
    }

    private static string GenerateCode()
    {
        var value = RandomNumberGenerator.GetInt32(0, 1000000);
        return value.ToString().PadLeft(6, '0');
    }
}