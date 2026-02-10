using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.CreateProject;

public static class CreateProjectEndpoint
{
    public static void MapCreateProject(this IEndpointRouteBuilder app)
    {
        // POST /projects
        app.MapPost("/", async (
            CreateProjectRequest request,
            [FromServices] List<Project> projects,
            [FromServices] User user,
            ProjectMapper mapper,
            IValidator<CreateProjectRequest> validator,
            ILogger<Program> logger) =>
        {
            await validator.ValidateAndThrowAsync(request);

            var project = new Project(request.ShortName, request.Name, user);
            projects.Add(project);

            logger.LogInformation(
                "Created project {Id} with short name {ShortName}, name {Name}",
                project.Id,
                project.ShortName,
                project.Name);

            return TypedResults.CreatedAtRoute(
                mapper.ToDto(project),
                EndpointNames.GetProject,
                new { id = project.Id });
        })
        .Produces<ProjectDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
