using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.ValueObjects;
using ErrorOr;
using FluentValidation;
using Newtonsoft.Json;

namespace DutyDock.Domain.Iam.User.ValueObjects;

public sealed class Language : ValueObject
{
    private const string DefaultLanguageValue = "en-US"; // English (United States)

    private static readonly List<string> SupportedLanguages = new()
    {
        DefaultLanguageValue
    };

    [JsonProperty] public string Value { get; private set; } = null!;

    private Language()
    {
    }

    private Language(string value)
    {
        Value = value;
    }

    internal static ErrorOr<Language> Create(string? value = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            value = DefaultLanguageValue;
        }

        var language = new Language(value);

        var validator = new LanguageValidator(SupportedLanguages);
        var result = validator.Validate(language);

        return result.ToErrorOr(language);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private class LanguageValidator : AbstractValidator<Language>
    {
        public LanguageValidator(IEnumerable<string> supportedLanguages)
        {
            RuleFor(model => model.Value)
                .NotEmpty()
                .Must(supportedLanguages.Contains)
                .WithError(BusinessErrors.User.UnknownLanguage);
        }
    }
}