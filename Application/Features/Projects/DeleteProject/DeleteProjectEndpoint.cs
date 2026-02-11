namespace ProjectTracker.Application.Features.Projects.DeleteProject;

public static class DeleteProjectEndpoint
{
    public static void MapDeleteProject(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{id}
        app.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteProjectCommand(id);

            await sender.Send(command);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent);
    }
}
