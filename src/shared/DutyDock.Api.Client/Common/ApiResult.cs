using System.Net;
using DutyDock.Api.Contracts.Common;

namespace DutyDock.Api.Client.Common;

public record ApiResult
{
    public static ApiResult ForSuccess(HttpStatusCode statusCode, Dictionary<string, string>? headers = null)
    {
        return new ApiResult(true, statusCode, headers, null);
    }

    public static ApiResult ForFailure(HttpStatusCode statusCode, Dictionary<string, string>? headers = null,
        Problem? problem = null)
    {
        return new ApiResult(false, statusCode, headers, problem);
    }

    protected ApiResult(bool success, HttpStatusCode statusCode, Dictionary<string, string>? headers, Problem? problem)
    {
        Success = success;
        StatusCode = statusCode;
        Headers = headers ?? new Dictionary<string, string>();
        Problem = problem;
    }

    public bool Success { get; }

    public Dictionary<string, string> Headers { get; }

    public HttpStatusCode StatusCode { get; }

    public Problem? Problem { get; }

    public override string ToString()
    {
        var output = $"ApiResult - Success: {Success}, Status: {StatusCode}";

        if (Problem != null)
        {
            output += $", {Problem}";
        }

        return output;
    }
}