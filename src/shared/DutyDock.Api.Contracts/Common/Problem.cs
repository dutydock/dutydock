namespace DutyDock.Api.Contracts.Common;

public sealed record Problem
{
    public string? Code { get; set; }

    public string? Description { get; set; }

    public List<ValidationError>? ValidationErrors { get; set; }

    public string? Trace { get; set; }

    public override string ToString()
    {
        var output = $"Problem: {Code}, {Description}";

        if (ValidationErrors != null && ValidationErrors.Count != 0)
        {
            var errors = ValidationErrors.Select(error => error.ToString()).ToList();
            var validationErrors = string.Join(',', errors);

            output += $", {validationErrors}";
        }

        if (Trace != null)
        {
            output += $", {Trace}";
        }

        return output;
    }
}