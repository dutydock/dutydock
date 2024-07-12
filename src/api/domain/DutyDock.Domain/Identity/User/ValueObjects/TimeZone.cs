using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.ValueObjects;
using ErrorOr;
using FluentValidation;
using Newtonsoft.Json;
using BusinessErrors = DutyDock.Domain.Identity.User.BusinessErrors;

namespace DutyDock.Domain.Identity.User.ValueObjects;

public sealed class TimeZone : ValueObject
{
    private const string DefaultTimeZoneValue = "UTC";

    private static List<string> _knownTimeZoneValues = new();

    [JsonProperty] public string Value { get; private set; } = null!;

    [JsonIgnore] public TimeZoneInfo Info => TimeZoneInfo.FindSystemTimeZoneById(Value);

    private TimeZone()
    {
    }

    private TimeZone(string value)
    {
        Value = value;
    }

    public static ErrorOr<TimeZone> Create(string? value = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            value = DefaultTimeZoneValue;
        }

        var timeZone = new TimeZone(value);

        var validator = new TimeZoneValidator(KnownTimeZoneValues);
        var result = validator.Validate(timeZone);

        return result.ToErrorOr(timeZone);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static IEnumerable<string> KnownTimeZoneValues
    {
        get
        {
            if (_knownTimeZoneValues.Count != 0)
            {
                return _knownTimeZoneValues.AsReadOnly();
            }

            var systemTimeZones = TimeZoneInfo.GetSystemTimeZones()
                .Where(zoneInfo => zoneInfo.HasIanaId)
                .ToList();

            var ianaIds = systemTimeZones.Select(info => info.Id).ToList();
            ianaIds.Add(DefaultTimeZoneValue);
            _knownTimeZoneValues = ianaIds.Distinct().ToList();

            return _knownTimeZoneValues.AsReadOnly();
        }
    }

    private class TimeZoneValidator : AbstractValidator<TimeZone>
    {
        public TimeZoneValidator(IEnumerable<string> knownTimeZoneValues)
        {
            RuleFor(model => model.Value)
                .NotEmpty()
                .Must(knownTimeZoneValues.Contains)
                .WithError(BusinessErrors.User.UnknownTimeZone);
        }
    }
}