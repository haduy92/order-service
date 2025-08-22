namespace Application.Models.Auth;

public record SignInRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

