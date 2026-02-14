using ProjectTracker.Application.Features.Issues.CreateIssue;
using ProjectTracker.Application.Features.Issues.GetIssue;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Api.Endpoints.Issues;

public record CreateIssueRequest(
    string Title,
    string? Description,
    Guid? AssigneeId,
    IssueType? Type,
    IssuePriority? Priority,
    Guid? ParentIssueId,
    DateTime? DueDate,
    int? EstimationMinutes);

internal static class CreateIssueEndpoint
{
    public static void MapCreateIssue(this IEndpointRouteBuilder app)
    {
        // POST /projects/{projectId}/issues
        app.MapPost("/", async (
            Guid projectId,
            CreateIssueRequest request,
            User user,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateIssueCommand(
                projectId,
                request.Title,
                request.Description,
                user,
                request.AssigneeId,
                request.Type,
                request.Priority,
                request.ParentIssueId,
                request.DueDate,
                request.EstimationMinutes);

            var issue = await sender.Send(command, ct);

            return TypedResults.CreatedAtRoute(
                issue,
                EndpointNames.GetIssue,
                new { projectId = issue.ProjectId, issueId = issue.Id });
        })
        .Produces<IssueDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
