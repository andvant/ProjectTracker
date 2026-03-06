using ProjectTracker.Application.Common;
using ProjectTracker.Application.Features.Users.UpdateUserGroups;

namespace ProjectTracker.Api.Endpoints.Users;

public record UpdateUserGroupsRequest(IReadOnlyCollection<Guid> GroupIds) : IRequest;

internal static class UpdateUserGroupsEndpoint
{
    public static void MapUpdateUserGroups(this IEndpointRouteBuilder app)
    {
        // PUT /users/{userId}/groups
        app.MapPut("/{userId:guid}/groups", async (
            Guid userId,
            UpdateUserGroupsRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new UpdateUserGroupsCommand(userId, request.GroupIds);

            await sender.Send(query, ct);

            return Results.NoContent();
        })
        .RequireAuthorization(p => p.RequireRole(Roles.Admin))
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
