using FlashCard.Application.Interfaces.Application;

namespace FlashCard.Api.Contexts;

public class CurrentUser : ICurrentUser
{
    public string? UserId { get; internal set; }
    public string? Username { get; internal set; }
    public bool IsAuthenticated { get; internal set; }

}
