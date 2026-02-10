namespace ProjectTracker.Features.Projects.DeleteProject;

public static class DeleteProjectEndpoint
{
    public static void MapDeleteProject(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{id}
        app.MapDelete("/{id}", async (
            Guid id,
            [FromServices] List<Project> projects,
            ILogger<Program> logger) =>
        {

            var project = projects.FirstOrDefault(p => p.Id == id);

            if (project is not null)
            {
                projects.Remove(project);

                logger.LogInformation(
                    "Updated project {Id} with short name {ShortName}, name {Name}",
                    project.Id,
                    project.ShortName,
                    project.Name);
            }

            return TypedResults.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent);
    }
}
