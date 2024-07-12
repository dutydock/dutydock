using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.ValueObjects;
using ErrorOr;
using FluentValidation;
using Newtonsoft.Json;
using BusinessErrors = DutyDock.Domain.Identity.User.BusinessErrors;

namespace DutyDock.Domain.Identity.User.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    private const int MaxLength = 150;

    [JsonProperty] public string Value { get; private set; } = null!;

    private EmailAddress()
    {
    }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static ErrorOr<EmailAddress> Create(string value)
    {
        var sanitizedValue = value.Trim().ToLowerInvariant();
        var emailAddress = new EmailAddress(sanitizedValue);

        var validator = new EmailAddressValidator();
        var result = validator.Validate(emailAddress);

        return result.ToErrorOr(emailAddress);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    private class EmailAddressValidator : AbstractValidator<EmailAddress>
    {
        public EmailAddressValidator()
        {
            RuleFor(model => model.Value)
                .NotEmpty()
                .WithError(BusinessErrors.User.EmailAddressRequired);
            RuleFor(model => model.Value)
                .EmailAddress()
                .WithError(BusinessErrors.User.EmailAddressInvalid);
            RuleFor(model => model.Value)
                .MaximumLength(MaxLength)
                .WithError(BusinessErrors.User.EmailAddressTooLong);
        }
    }
}