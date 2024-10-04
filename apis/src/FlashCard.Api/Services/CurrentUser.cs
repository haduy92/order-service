using FlashCard.Application.Interfaces.Application;

namespace FlashCard.Api.Services;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    public int UserId { get; set; }
}
