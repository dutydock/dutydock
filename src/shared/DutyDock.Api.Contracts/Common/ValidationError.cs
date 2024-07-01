namespace DutyDock.Api.Contracts.Common;

public sealed record ValidationError(string? Code, string? Description)
{
    public override string ToString()
    {
        return $"ValidationError: {Code}, {Description}";
    }
}