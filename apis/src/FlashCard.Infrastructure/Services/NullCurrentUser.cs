using FlashCard.Application.Interfaces.Application;

namespace FlashCard.Infrastructure.Services;

public class NullCurrentUser : ICurrentUser
{
    public string? UserId => string.Empty;
    public string? Username => string.Empty;
    public bool IsAuthenticated => false;

    public static NullCurrentUser Instance => new NullCurrentUser();
}
