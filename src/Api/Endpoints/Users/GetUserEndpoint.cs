using ProjectTracker.Application.Features.Users.GetUser;

namespace UserTracker.Api.Endpoints.Users;

internal static class GetUserEndpoint
{
    public static void MapGetUser(this IEndpointRouteBuilder app)
    {
        // GET /users/{userId}
        app.MapGet("/{userId}", async (
            Guid userId,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetUserQuery(userId);

            var user = await sender.Send(query, ct);

            return TypedResults.Ok(user);
        })
        .Produces<UserDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
