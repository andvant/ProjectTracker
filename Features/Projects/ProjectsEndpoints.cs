using ProjectTracker.Features.Projects.CreateProject;
using ProjectTracker.Features.Projects.DeleteProject;
using ProjectTracker.Features.Projects.GetProject;
using ProjectTracker.Features.Projects.GetProjects;
using ProjectTracker.Features.Projects.UpdateProject;

namespace ProjectTracker.Features.Projects;

public static class ProjectsEndpoints
{
    public static void MapProjects(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects");

        group.MapGetProject();
        group.MapGetProjects();
        group.MapCreateProject();
        group.MapUpdateProject();
        group.MapDeleteProject();
    }
}
