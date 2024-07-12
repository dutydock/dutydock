using DutyDock.Domain.Common.Errors;
using ErrorOr;
using FluentValidation;
using Newtonsoft.Json;

namespace DutyDock.Domain.Common.Models.ValueObjects;

public class Name : ValueObject
{
    private const int MaxLength = 50;
    
    private static readonly Error NameRequired =
        Error.Validation("Name.Required", "Name is required");

    private static readonly Error NameTooLong =
        Error.Validation("Name.TooLong", "Name is too long");
    
    [JsonProperty] public string Value { get; private set; } = null!;

    [JsonProperty] public string OrderValue { get; private set; } = null!;
    
    private Name()
    {
    }

    private Name(string value)
    {
        Value = value;

        if (value != null!)
        {
            OrderValue = value.ToUpperInvariant();
        }
    }

    public static ErrorOr<Name> Create(string? name)
    {
        var sanitizedValue = name?.Trim();
        var text = new Name(sanitizedValue!);
        
        var validator = new NameValidator();
        var result = validator.Validate(text);

        return result.ToErrorOr(text);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }
    
    private class NameValidator : AbstractValidator<Name>
    {
        public NameValidator()
        {
            RuleFor(model => model.Value)
                .NotEmpty()
                .WithError(NameRequired);

            RuleFor(model => model.Value)
                .MaximumLength(MaxLength)
                .WithError(NameTooLong);
        }
    }
}