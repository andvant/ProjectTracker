using ProjectTracker.Application.Features.Projects.AddMember;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class AddMemberEndpoint
{
    public static void MapAddMember(this IEndpointRouteBuilder app)
    {
        // PUT /projects/{projectId}/members/{memberId}
        app.MapPut("/{projectId:guid}/members/{memberId:guid}", async (
            Guid projectId,
            Guid memberId,
            ISender sender,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var command = new AddMemberCommand(projectId, memberId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
