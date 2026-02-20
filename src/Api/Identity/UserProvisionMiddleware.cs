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

        var externalId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var username = user.FindFirstValue("preferred_username")!;
        var email = user.FindFirstValue(ClaimTypes.Email)!;

        var command = new ProvisionUserCommand(Guid.Parse(externalId), username, email);

        await _sender.Send(command);

        await next(context);
    }
}
