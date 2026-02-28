using ProjectTracker.Application.Features.Users.GetUserGroups;

namespace UserTracker.Api.Endpoints.Users;

internal static class GetUserGroupsEndpoint
{
    public static void MapGetUserGroups(this IEndpointRouteBuilder app)
    {
        // GET /users/{userId}/groups
        app.MapGet("/{userId:guid}/groups", async (
            Guid userId,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetUserGroupsQuery(userId);

            var userGroups = await sender.Send(query, ct);

            return TypedResults.Ok(userGroups);
        })
        .Produces<IReadOnlyCollection<UserGroupDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
