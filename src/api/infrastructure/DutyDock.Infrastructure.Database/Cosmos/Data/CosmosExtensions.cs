using System.Net;
using DutyDock.Infrastructure.Database.Common.Exceptions;
using Microsoft.Azure.Cosmos;

namespace DutyDock.Infrastructure.Database.Cosmos.Data;

public static class CosmosExtensions
{
    /*
     * https://learn.microsoft.com/en-us/rest/api/cosmos-db/http-status-codes-for-cosmosdb
     */
    public static Exception Evaluate(this CosmosException error, string? id = null, string? etag = null)
    {
        return Evaluate(error.StatusCode, id, etag);
    }

    public static Exception Evaluate(this HttpStatusCode statusCode, string? id = null, string? etag = null)
    {
        return statusCode switch
        {
            // The operation is attempting to act on a resource that no longer exists.
            HttpStatusCode.NotFound => new NotFoundException(
                $"Object not found for Id: {id ?? string.Empty} / ETag: {etag}"),
            HttpStatusCode.NotModified => new NotModifiedException(
                $"Object not modified. Id: {id ?? string.Empty} / ETag: {etag}"),
            // The ID provided for a resource on a PUT or POST operation has been taken by an existing resource.
            HttpStatusCode.Conflict => new ConflictException(
                $"Object conflict detected. Id: {id ?? string.Empty} / ETag: {etag}"),
            // The operation specified an eTag that is different from the version available at the server.
            HttpStatusCode.PreconditionFailed => new PreconditionFailedException(
                $"Object mid-air collision detected. Id: {id ?? string.Empty} / ETag: {etag}"),
            // The collection has exceeded the provisioned throughput limit.
            HttpStatusCode.TooManyRequests => new TooManyRequestsException(
                "Too many requests occurred. Try again later)"),
            _ => new Exception($"Cosmos Exception (Status Code: {statusCode})")
        };
    }

    public static string Stringify(this TransactionalBatchResponse response)
    {
        return $"{response.StatusCode} {response.ErrorMessage}";
    }

    public static string Stringify(this TransactionalBatchOperationResult result)
    {
        return $"{result.StatusCode} {result.ETag}";
    }
}