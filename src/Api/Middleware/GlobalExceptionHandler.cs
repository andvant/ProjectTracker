using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Application.Exceptions;
using ProjectTracker.Domain.Exceptions;

namespace ProjectTracker.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
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

        var problemDetails = new ProblemDetails()
        {
            Title = exception.Message,
            Detail = exception.StackTrace,
            Status = httpContext.Response.StatusCode,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Extensions =
            {
                ["traceId"] = Activity.Current?.TraceId.ToString(),
                ["requestId"] = httpContext.TraceIdentifier,
            }
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, token);

        return true;
    }
}
