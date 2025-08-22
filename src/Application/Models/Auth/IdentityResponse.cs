namespace Application.Models.Auth;

public record IdentityResponse
{
    public string? UserId { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}

