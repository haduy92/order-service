using Application.Contracts.Application;

namespace Api.Contexts;

public class CurrentUser : ICurrentUser
{
    public string? UserId { get; internal set; }
    public string? Username { get; internal set; }
    public bool IsAuthenticated { get; internal set; }

    public override string ToString()
    {
        return $"UserId: {UserId}, Username: {Username}, IsAuthenticated: {IsAuthenticated}";
    }
}

