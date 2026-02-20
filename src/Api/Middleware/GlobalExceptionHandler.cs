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
            case // problem with URL
                ProjectNotFoundException or
                IssueNotFoundException or
                UserNotFoundException or
                MemberNotFoundException or
                WatcherNotFoundException or
                NewOwnerNotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                break;
            case // problem with body
                AssigneeNotFoundException or
                ParentIssueNotFoundException or
                ParentIssueWrongProjectException or
                ParentIssueWrongTypeException or
                ChildIssueWrongTypeException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case // violated constraints/invariants
                ProjectKeyAlreadyExistsException or
                AssigneeNotMemberException or
                CantRemoveProjectOwnerException:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            case ApplicationException or DomainException:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                logger.LogError(exception, "An unhandled Application or Domain exception occurred.");
                break;
            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                logger.LogError(exception, "An unhandled exception occurred.");
                break;
        }

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = environment.IsDevelopment() ? exception.Message : "An unknown error occurred.",
                Detail = environment.IsDevelopment() ? exception.ToString() : null,
                Status = httpContext.Response.StatusCode,
            }
        };

        return await problemDetailsService.TryWriteAsync(context);
    }
}
