using FluentValidation;

namespace DutyDock.Web.Core.Helpers;

public class BaseModelValidator<T> : AbstractValidator<T>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var normalizedPropertyName = 
            propertyName.Contains('.') ? propertyName.Split('.').Last() : propertyName;

        var result =
            await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model,
                x => x.IncludeProperties(normalizedPropertyName)));
        
        return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
    };
}