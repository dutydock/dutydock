using System.Numerics;

namespace DutyDock.Domain.Common.Services;

/**
 * This class generates random/unique ID's using UUID v4 (https://www.ietf.org/rfc/rfc4122.txt 4.4),
 * but encodes them using base36 (https://en.wikipedia.org/wiki/Base36).
 *
 * Base36 encoding condenses the canonical representation of a guid,
 * for example 89624960-6c9c-484c-9515-1bf30679a84f to 994pckaj5q9ga6q5aaqclg47u.
 * As this encoding only uses numbers and lower-case letters,
 * each identifier is also a valid DNS entry.
 */
public static class IdentityProvider
{
    private const uint MinLength = 22;
    private const uint MaxLength = 25;

    public static string New()
    {
        var bigInt = GuidToBigIntPositive(Guid.NewGuid());
        return Base36.Encode(bigInt);
    }

    public static bool IsValid(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        if (id.Length < MinLength || id.Length > MaxLength)
        {
            return false;
        }

        var hasValidCharacters = id.All(c => Base36.CharList.Contains(c));

        if (!hasValidCharacters)
        {
            return false;
        }

        try
        {
            var guidBytes = Base36.Decode(id);
            var guid = BigIntToGuidPositive(guidBytes);

            var result = New(guid);

            return result == id;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static string New(Guid guid)
    {
        var bigInt = GuidToBigIntPositive(guid);
        return Base36.Encode(bigInt);
    }

    private static BigInteger GuidToBigIntPositive(Guid guid)
    {
        var guidBytes = guid.ToByteArray();

        // Pad extra 0x00 byte so value is handled as positive integer
        var positiveGuidBytes = new byte[guidBytes.Length + 1];
        Array.Copy(guidBytes, positiveGuidBytes, guidBytes.Length);

        return new BigInteger(positiveGuidBytes);
    }

    private static Guid BigIntToGuidPositive(BigInteger bigint)
    {
        // Allocate extra byte to store the large positive integer
        var positiveBytes = new byte[17];
        bigint.ToByteArray().CopyTo(positiveBytes, 0);

        // Strip the extra byte so Guid can handle it
        var bytes = new byte[16];
        Array.Copy(positiveBytes, bytes, bytes.Length);

        return new Guid(bytes);
    }
}