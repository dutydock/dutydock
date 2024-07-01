using DutyDock.Api.Contracts.Common;
using ErrorOr;

namespace DutyDock.Api.Shared;

public static class ErrorExtensions
{
    public static Problem AsProblem(this Error error,
        List<ValidationError>? validationErrors = null, Exception? ex = null)
    {
        return new Problem
        {
            Code = error.Code,
            Description = error.Description,
            ValidationErrors = validationErrors,
            Trace = ex == null ? null : $"{ex.Message} {ex.StackTrace}"
        };
    }
}