using System.Net;
using DutyDock.Api.Contracts.Common;

namespace DutyDock.Api.Client.Common;

public sealed record ApiDataResult<T> : ApiResult
{
    public static ApiDataResult<T> ForSuccess(T? data, HttpStatusCode statusCode,
        Dictionary<string, string>? headers = null)
    {
        return new ApiDataResult<T>(true, statusCode, headers, null, data);
    }

    public new static ApiDataResult<T> ForFailure(HttpStatusCode statusCode, Dictionary<string, string>? headers = null,
        Problem? problem = null)
    {
        return new ApiDataResult<T>(false, statusCode, headers, problem, default);
    }

    private ApiDataResult(bool success, HttpStatusCode statusCode, Dictionary<string, string>? headers,
        Problem? problem,
        T? data) : base(success,
        statusCode, headers, problem)
    {
        Data = data;
    }

    public T? Data { get; }

    public override string ToString()
    {
        var output = base.ToString();

        if (Data != null)
        {
            output += $", Data: {Data}";
        }

        return output;
    }
}