using System.Numerics;

namespace DutyDock.Domain.Common.Services;

internal static class Base36
{
    public const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

    public static BigInteger Decode(string input)
    {
        var reversed = input.ToLower().Reverse();

        var result = BigInteger.Zero;
        var pos = 0;

        foreach (var c in reversed)
        {
            result = BigInteger.Add(
                result,
                BigInteger.Multiply(CharList.IndexOf(c),
                    BigInteger.Pow(36, pos)));
            pos++;
        }

        return result;
    }

    public static string Encode(BigInteger input)
    {
        if (input.Sign < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(input), input, "input cannot be negative");
        }

        var result = new Stack<char>();

        while (!input.IsZero)
        {
            var index = (int)(input % 36);
            result.Push(CharList[index]);
            input = BigInteger.Divide(input, 36);
        }

        return new string(result.ToArray());
    }
}