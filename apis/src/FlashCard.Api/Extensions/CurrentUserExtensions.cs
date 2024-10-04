using System.Security.Claims;
using FlashCard.Application.Interfaces.Application;

namespace FlashCard.Api.Extensions;

public static class CurrentUserExtensions
{
    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var user = context.User;
            var currentUser = context.RequestServices.GetRequiredService<ICurrentUserInitializer>();

            currentUser.UserId = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

            await next();
        });

        return app;
    }
}
