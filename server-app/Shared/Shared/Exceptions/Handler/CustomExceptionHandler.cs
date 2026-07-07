using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Shared.Exceptions.Handler
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (statusCode, title, detail) = MapException(exception);

            var problem = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Type = exception.GetType().Name,
                Instance = httpContext.Request.Path
            };

            AttachExtensions(problem, exception);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }

        private static (int StatusCode, string Title, string Detail) MapException(Exception ex) => ex switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed", "One or more validation errors occurred."),
            BadRequestException br => (StatusCodes.Status400BadRequest, "Bad request", br.Message),
            NotFoundException nf => (StatusCodes.Status404NotFound, "Not found", nf.Message),
            BusinessRuleException br => (StatusCodes.Status422UnprocessableEntity, "Business rule violation", br.Message),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Concurrency conflict",
                "The resource was modified by another process. Reload and try again."),
            InternalServerException iex => (StatusCodes.Status500InternalServerError, "Internal server error", iex.Message),
            _ => (StatusCodes.Status500InternalServerError, "Server error", ex.Message)
        };

        private static void AttachExtensions(ProblemDetails problem, Exception ex)
        {
            switch (ex)
            {
                case ValidationException fv:
                    problem.Extensions["errors"] = fv.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                    break;

                case BadRequestException br when !string.IsNullOrWhiteSpace(br.Details):
                    problem.Extensions["details"] = br.Details;
                    break;

                case BusinessRuleException biz when biz.Payload is not null:
                    problem.Extensions["payload"] = biz.Payload;
                    break;
            }
        }
    }
}
