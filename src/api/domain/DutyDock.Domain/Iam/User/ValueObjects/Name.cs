using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.ValueObjects;
using ErrorOr;
using FluentValidation;
using Newtonsoft.Json;

namespace DutyDock.Domain.Iam.User.ValueObjects;

public sealed class Name : ValueObject
{
    private const int MaxLength = 50;

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

    public static Name Create(EmailAddress emailAddress)
    {
        return new Name(emailAddress.Value);
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
                .WithError(BusinessErrors.User.NameRequired);

            RuleFor(model => model.Value)
                .MaximumLength(MaxLength)
                .WithError(BusinessErrors.User.NameTooLong);
        }
    }
}