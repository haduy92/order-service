namespace Application.Models;

public record RefreshTokenDto
{
    public required string RefreshToken { get; init; }
    public required DateTime Expires { get; init; }
}

