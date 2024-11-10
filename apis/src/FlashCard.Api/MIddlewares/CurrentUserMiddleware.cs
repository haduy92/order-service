using System.Security.Claims;
using FlashCard.Api.Contexts;
using FlashCard.Application.Interfaces.Application;

namespace FlashCard.Api.MIddlewares;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var _currentUser = currentUser as CurrentUser;
            if (_currentUser is not null)
            {
                _currentUser.UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _currentUser.Username = context.User.FindFirstValue(ClaimTypes.Email);
                _currentUser.IsAuthenticated = true;
            }
        }

        await _next(context);
    }
}
