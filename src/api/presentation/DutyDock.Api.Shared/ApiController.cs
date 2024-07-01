using System.Net;
using DutyDock.Api.Contracts.Common;
using DutyDock.Application.Common;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Throw;

namespace DutyDock.Api.Shared;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected static IActionResult Problem(List<Error> errors)
    {
        errors.ThrowIfNull().IfEmpty();

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        var firstError = errors.First();
        return Problem(firstError);
    }

    private static ObjectResult Problem(Error error)
    {
        var statusCode = error.NumericType switch
        {
            (int)ErrorType.NotFound => StatusCodes.Status404NotFound,
            (int)ErrorType.Conflict => StatusCodes.Status409Conflict,
            CustomErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new Problem
        {
            Code = error.Code,
            Description = error.Description
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }

    private static ObjectResult ValidationProblem(IEnumerable<Error> errors)
    {
        var validationErrors = errors
            .Select(error => new ValidationError(error.Code, error.Description))
            .ToList();

        var problem = Error.Validation().AsProblem(validationErrors);

        return new ObjectResult(problem)
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
    }
}