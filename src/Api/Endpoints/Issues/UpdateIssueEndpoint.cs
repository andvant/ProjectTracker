using ProjectTracker.Application.Features.Issues.UpdateIssue;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Api.Endpoints.Issues;

public record UpdateIssueRequest(
    string Title,
    string? Description,
    Guid? AssigneeId,
    IssueStatus Status,
    IssuePriority Priority,
    DateTimeOffset? DueDate,
    int? EstimationMinutes
);

internal static class UpdateIssueEndpoint
{
    public static void MapUpdateIssue(this IEndpointRouteBuilder app)
    {
        // PUT /projects/{projectId}/issues/{issueId}
        app.MapPut("/{issueId:guid}", async (
            Guid projectId,
            Guid issueId,
            UpdateIssueRequest request,
            ISender sender,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var command = new UpdateIssueCommand(
                projectId,
                issueId,
                request.Title,
                request.Description,
                request.AssigneeId,
                request.Status,
                request.Priority,
                request.DueDate,
                request.EstimationMinutes);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();
    }
}
