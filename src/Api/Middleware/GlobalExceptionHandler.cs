using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Application.Exceptions;
using ProjectTracker.Domain.Exceptions;

namespace ProjectTracker.Api.Middleware;

internal class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        switch (exception)
        {
            case ProjectNotFoundException or IssueNotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                break;
            case AssigneeNotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case AssigneeNotMemberException:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                logger.LogError(exception, "An unhandled exception occurred");
                break;
        }

        var context = new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails()
            {
                Title = exception.Message,
                Detail = exception.StackTrace, // TODO: remove in production env
                Status = httpContext.Response.StatusCode,
            }
        };

        return await problemDetailsService.TryWriteAsync(context);
    }
}
