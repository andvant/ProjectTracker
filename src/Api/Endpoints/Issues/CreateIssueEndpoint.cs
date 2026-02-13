using ProjectTracker.Application.Features.Issues.Common;
using ProjectTracker.Application.Features.Issues.CreateIssue;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Api.Endpoints.Issues;

public record CreateIssueRequest(string Title, Guid? AssigneeId, IssueType? Type, IssuePriority? Priority);

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
            var command = new CreateIssueCommand(projectId, request.Title, user,
                request.AssigneeId, request.Type, request.Priority);

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
