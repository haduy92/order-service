namespace FlashCard.Application.Models;

public record SignInRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
