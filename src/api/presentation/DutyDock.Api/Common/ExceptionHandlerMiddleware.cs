using System.Net;
using DutyDock.Api.Contracts.Common;
using DutyDock.Api.Shared;
using DutyDock.Application.Common.Exceptions;
using DutyDock.Application.Common.Interfaces;
using DutyDock.Application.Common.Interfaces.Services;
using ErrorOr;
using Newtonsoft.Json;

namespace DutyDock.Api.Common;

public static class ExceptionHandlerMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }

    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IEnvironmentProvider _environment;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IEnvironmentProvider environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ForbiddenException:
                        Response(context, HttpStatusCode.Forbidden);
                        break;
                    case UnauthorizedException:
                        Response(context, HttpStatusCode.Unauthorized);
                        break;
                    default:
                        await UnexpectedResponse(context, ex);
                        break;
                }
            }
        }

        private async Task UnexpectedResponse(HttpContext context, Exception ex)
        {
            _logger.LogCritical(ex, "{Message}", ex.Message);
            await ProblemResponse(context, HttpStatusCode.InternalServerError, Error.Unexpected(), ex);
        }

        private async Task ProblemResponse(
            HttpContext context, HttpStatusCode statusCode, Error error, Exception? exception = null)
        {
            // Do not print stack traces in hosted environments
            var problem = _environment.IsVirtual ? error.AsProblem(ex: exception) : error.AsProblem();

            var json = JsonConvert.SerializeObject(problem, JsonSettings.Get());
            var response = context.Response;

            response.StatusCode = (int)statusCode;
            response.ContentType = "application/problem+json";

            await response.WriteAsync(json);
        }

        private static void Response(HttpContext context, HttpStatusCode statusCode)
        {
            var response = context.Response;
            response.StatusCode = (int)statusCode;
        }
    }
}