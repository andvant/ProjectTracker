namespace ProjectTracker.Features.Projects.UpdateProject;

public static class UpdateProjectEndpoint
{
    public static void MapUpdateProject(this IEndpointRouteBuilder app)
    {
        // PUT /projects/{id}
        app.MapPut("/{id}", async (
            Guid id,
            UpdateProjectRequest request,
            [FromServices] List<Project> projects,
            IValidator<UpdateProjectRequest> validator,
            ILogger<Program> logger) =>
        {
            await validator.ValidateAndThrowAsync(request);

            var project = projects.FirstOrDefault(p => p.Id == id);

            if (project is not null)
            {
                project.Name = request.Name;
                project.Description = request?.Description;

                logger.LogInformation(
                    "Updated project {Id} with short name {ShortName}, name {Name}",
                    project.Id,
                    project.ShortName,
                    project.Name);
            }

            return TypedResults.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem();
    }
}
