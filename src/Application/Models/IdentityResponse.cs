namespace Application.Models;

public record IdentityResponse : ResponseBase
{
    public string? UserId { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}

