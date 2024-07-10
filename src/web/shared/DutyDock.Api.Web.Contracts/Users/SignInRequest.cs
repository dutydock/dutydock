namespace DutyDock.Api.Web.Contracts.Users;

public sealed record SignInRequest
{
    public string? EmailAddress { get; set; }

    public string? Password { get; set; }

    public bool IsPersisted { get; set; }
}