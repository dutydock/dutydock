using System.Net;

namespace DutyDock.Api.Client.Common;

public static class ApiResultExtensions
{
    public static bool HasValidationError<T>(this ApiDataResult<T>? result, string errorCode)
    {
        return HasValidationError((ApiResult?)result, errorCode);
    }

    public static bool IsSuccessWithData<T>(this ApiDataResult<T>? result)
    {
        return result is { Success: true } && result.Data != null;
    }

    public static bool IsFailure(this ApiResult? result)
    {
        return result is { Success: false } && result.StatusCode != HttpStatusCode.Forbidden &&
               result.StatusCode != HttpStatusCode.Unauthorized;
    }

    public static bool HasValidationError(this ApiResult? result, string errorCode)
    {
        if (result == null)
        {
            return false;
        }

        if (result.StatusCode != HttpStatusCode.BadRequest)
        {
            return false;
        }

        var validationErrors = result.Problem?.ValidationErrors;
        
        if (validationErrors == null || !validationErrors.Any())
        {
            return false;
        }

        return validationErrors.FirstOrDefault(error => error.Code == errorCode) != null;
    }
}