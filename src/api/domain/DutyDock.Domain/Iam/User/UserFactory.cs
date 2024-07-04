using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Services;
using DutyDock.Domain.Iam.User.Services;
using DutyDock.Domain.Iam.User.ValueObjects;
using ErrorOr;
using TimeZone = DutyDock.Domain.Iam.User.ValueObjects.TimeZone;

namespace DutyDock.Domain.Iam.User;

public static class UserFactory
{
    public static ErrorOr<User> CreateFull(
        string fullName,
        string emailAddress,
        string password,
        IUserPasswordHasher passwordHasher,
        string? culture = null,
        string? timeZone = null,
        string? language = null)
    {
        var fullNameResult = Name.Create(fullName);
        var emailAddressResult = EmailAddress.Create(emailAddress);
        var passwordResult = Password.Create(password, passwordHasher);
        var cultureResult = Culture.Create(culture);
        var timeZoneResult = TimeZone.Create(timeZone);
        var languageResult = Language.Create(language);

        var errors = fullNameResult.ToErrors()
            .Append(emailAddressResult)
            .Append(passwordResult)
            .Append(cultureResult)
            .Append(timeZoneResult)
            .Append(languageResult);

        if (errors.Count != 0)
        {
            return errors;
        }

        var id = IdentityProvider.New();
        var securityStamp = SecurityStamp.Create();

        return new User(
            id,
            emailAddressResult.Value,
            fullNameResult.Value,
            passwordResult.Value,
            securityStamp,
            cultureResult.Value,
            timeZoneResult.Value,
            languageResult.Value);
    }

    public static ErrorOr<User> CreateMinimal(
        string emailAddress,
        string? fullName = null,
        string? culture = null,
        string? timeZone = null,
        string? language = null)
    {
        var emailAddressResult = EmailAddress.Create(emailAddress);
        var cultureResult = Culture.Create(culture);
        var timeZoneResult = TimeZone.Create(timeZone);
        var languageResult = Language.Create(language);

        var errors = emailAddressResult.ToErrors()
            .Append(cultureResult)
            .Append(timeZoneResult)
            .Append(languageResult);

        if (errors.Count != 0)
        {
            return errors;
        }

        Name name;

        if (fullName != null)
        {
            var fullNameResult = Name.Create(fullName);

            if (fullNameResult.IsError)
            {
                return fullNameResult.Errors;
            }

            name = fullNameResult.Value;
        }
        else
        {
            name = Name.Create(emailAddressResult.Value);
        }

        var id = IdentityProvider.New();
        var securityStamp = SecurityStamp.Create();

        return new User(
            id,
            emailAddressResult.Value,
            name,
            securityStamp,
            cultureResult.Value,
            timeZoneResult.Value,
            languageResult.Value);
    }
}