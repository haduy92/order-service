using System.Security.Claims;
using Api.Contexts;
using Application.Contracts.Application;

namespace Api.Middlewares;

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
            var user = (CurrentUser)currentUser;
            user.UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            user.Username = context.User.FindFirst(ClaimTypes.Name)?.Value;
            user.IsAuthenticated = true;
        }
        
        await _next(context);
    }
}

