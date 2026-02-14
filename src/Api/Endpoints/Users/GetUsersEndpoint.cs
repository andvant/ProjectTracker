using ProjectTracker.Application.Features.Users.GetUsers;

namespace UserTracker.Api.Endpoints.Users;

internal static class GetUsersEndpoint
{
    public static void MapGetUsers(this IEndpointRouteBuilder app)
    {
        // GET /users
        app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var query = new GetUsersQuery();

            var users = await sender.Send(query, ct);

            return TypedResults.Ok(users);
        })
        .Produces<List<UsersDto>>(StatusCodes.Status200OK);
    }
}
