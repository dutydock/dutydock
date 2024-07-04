using System.Globalization;
using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.ValueObjects;
using ErrorOr;
using FluentValidation;
using Newtonsoft.Json;

namespace DutyDock.Domain.Iam.User.ValueObjects;

public sealed class Culture : ValueObject
{
    private const string DefaultCultureValue = "en-US"; // English (United States)

    private static List<string> _knownCultureValues = new();

    [JsonProperty] public string Value { get; private set; } = null!;

    private Culture()
    {
    }

    private Culture(string value)
    {
        Value = value;
    }

    internal static ErrorOr<Culture> Create(string? value = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            value = DefaultCultureValue;
        }

        var culture = new Culture(value);

        var validator = new CultureValidator(KnownCultureValues);
        var result = validator.Validate(culture);

        return result.ToErrorOr(culture);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static IEnumerable<string> KnownCultureValues
    {
        get
        {
            if (_knownCultureValues.Count != 0)
            {
                return _knownCultureValues.AsReadOnly();
            }

            var cultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            _knownCultureValues = cultureInfos.Select(info => info.Name).ToList();

            return _knownCultureValues.AsReadOnly();
        }
    }

    private class CultureValidator : AbstractValidator<Culture>
    {
        public CultureValidator(IEnumerable<string> knownCultureValues)
        {
            RuleFor(model => model.Value)
                .NotEmpty()
                .Must(knownCultureValues.Contains)
                .WithError(BusinessErrors.User.UnknownCulture);
        }
    }
}