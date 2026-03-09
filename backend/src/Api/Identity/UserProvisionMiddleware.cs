using System.Security.Claims;
using ProjectTracker.Application.Features.Users.ProvisionUser;

namespace ProjectTracker.Api.Identity;

public class UserProvisionMiddleware : IMiddleware
{
    private readonly ISender _sender;

    public UserProvisionMiddleware(ISender sender)
    {
        _sender = sender;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var id = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var email = user.FindFirstValue(ClaimTypes.Email)!;
        var username = user.FindFirstValue("preferred_username")!;
        var fullName = user.FindFirstValue("name")!;

        var command = new ProvisionUserCommand(Guid.Parse(id), username, email, fullName);

        await _sender.Send(command);

        await next(context);
    }
}
