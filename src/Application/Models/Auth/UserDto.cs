namespace Application.Models.Auth;

public record UserDto
{
    public required string Id { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
}

