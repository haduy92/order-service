namespace FlashCard.Application.Models;

public record UserDto
{
    public int Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
