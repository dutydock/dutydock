using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.ValueObjects;
using DutyDock.Domain.Iam.User.Services;
using ErrorOr;
using FluentValidation;
using Newtonsoft.Json;

namespace DutyDock.Domain.Iam.User.ValueObjects;

public sealed class Password : ValueObject
{
    private const int MinLength = 10;
    private const int MaxLength = 50;

    [JsonIgnore] private string? Value { get; }

    [JsonProperty] public string? HashedValue { get; private set; }
    
    private Password()
    {
    }

    private Password(string value)
    {
        Value = value;
    }
    
    public static ErrorOr<Password> Create(string? value, IUserPasswordHasher passwordHasher)
    {
        var password = new Password(value!);

        var validator = new PasswordValidator();
        var result = validator.Validate(password);

        if (result.Errors.Count != 0)
        {
            return result.ToErrorOr<Password>();
        }

        password.HashedValue = passwordHasher.Hash(value!);
        return password;
    }
 
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedValue;
    }
 
    public override string ToString()
    {
        return HashedValue ?? string.Empty;
    }
    
    private class PasswordValidator : AbstractValidator<Password>
    {
        public PasswordValidator()
        {
            RuleFor(model=> model.Value)
                .NotNull()
                .NotEmpty()
                .WithError(BusinessErrors.User.PasswordRequired);
            RuleFor(model => model.Value)
                .MinimumLength(MinLength)
                .WithError(BusinessErrors.User.PasswordTooShort);
            RuleFor(model => model.Value)
                .MaximumLength(MaxLength)
                .WithError(BusinessErrors.User.PasswordTooLong);
        }
    }
}