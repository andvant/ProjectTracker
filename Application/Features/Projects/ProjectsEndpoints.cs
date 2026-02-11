using ProjectTracker.Application.Features.Projects.CreateProject;
using ProjectTracker.Application.Features.Projects.DeleteProject;
using ProjectTracker.Application.Features.Projects.GetProject;
using ProjectTracker.Application.Features.Projects.GetProjects;
using ProjectTracker.Application.Features.Projects.UpdateProject;

namespace ProjectTracker.Application.Features.Projects;

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
