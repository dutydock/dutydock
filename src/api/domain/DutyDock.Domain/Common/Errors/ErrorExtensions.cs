using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Throw;

namespace DutyDock.Domain.Common.Errors;

public static class ErrorExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Error? error)
    {
        error.ThrowIfNull();
        return rule.WithErrorCode(error.Value.Code).WithMessage(error.Value.Description);
    }

    public static ErrorOr<T> ToErrorOr<T>(this ValidationResult validationResult, T data)
    {
        validationResult.ThrowIfNull();

        if (validationResult.IsValid)
        {
            return data;
        }

        return validationResult.Errors.ConvertAll(
            error => Error.Validation(error.ErrorCode, error.ErrorMessage));
    }

    public static ErrorOr<T> ToErrorOr<T>(this ValidationResult validationResult)
    {
        validationResult.ThrowIfNull();

        if (validationResult.IsValid)
        {
            throw new ArgumentException(null, nameof(validationResult));
        }

        return validationResult.Errors.ConvertAll(
            error => Error.Validation(error.ErrorCode, error.ErrorMessage));
    }

    public static List<Error> ToErrors(this IErrorOr source)
    {
        return source is { IsError: true, Errors: not null } ? source.Errors : new List<Error>();
    }

    public static List<Error> Append(this List<Error> errors, IErrorOr source)
    {
        if (source is { IsError: true, Errors: not null })
        {
            errors.AddRange(source.Errors);
        }

        return errors;
    }

    public static List<Error> ConcatErrors(this IErrorOr source, params IErrorOr[] targets)
    {
        var errors = source.ToErrors();

        foreach (var target in targets)
        {
            errors.Append(target);
        }

        return errors;
    }

    public static ErrorOr<T> Cast<T, TS>(this ErrorOr<TS> result) where TS : T
    {
        if (result.IsError)
        {
            return (ErrorOr<T>)result.Errors;
        }

        return result.Value;
    }
}