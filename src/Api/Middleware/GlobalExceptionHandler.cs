using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Application.Exceptions;
using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Exceptions;

namespace ProjectTracker.Api.Middleware;

internal class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        switch (exception)
        {
            case // resource from URL not found
                ProjectNotFoundException or
                IssueNotFoundException or
                UserNotFoundException or
                MemberNotFoundException or
                WatcherNotFoundException or
                NewOwnerNotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                break;
            case // resource from body not found
                AssigneeNotFoundException or
                ParentIssueNotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case
                ProjectKeyAlreadyExistsException or
                AssigneeNotMemberException or
                CantRemoveProjectOwnerException:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            case ApplicationException or DomainException:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                logger.LogError(exception, "An unhandled Application or Domain exception occurred");
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
                Detail = environment.IsDevelopment() ? exception.ToString() : null,
                Status = httpContext.Response.StatusCode,
            }
        };

        return await problemDetailsService.TryWriteAsync(context);
    }
}
