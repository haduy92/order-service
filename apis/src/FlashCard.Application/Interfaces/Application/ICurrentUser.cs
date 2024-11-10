namespace FlashCard.Application.Interfaces.Application;

public interface ICurrentUser
{
    string? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
}
