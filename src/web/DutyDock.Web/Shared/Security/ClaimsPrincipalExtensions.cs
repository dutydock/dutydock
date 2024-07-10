using System.Security.Claims;

namespace DutyDock.Web.Shared.Security;

public static class ClaimsPrincipalExtensions
{
    public static string ToClaims(this ClaimsPrincipal user)
    {
        var claims = string.Join(", ", user.Claims.Select(claim => $"{claim.Type}: {claim.Value}"));
        return $"({claims})";
    }
}