namespace Application.Models.Auth;

public record SignUpRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

