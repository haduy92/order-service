namespace EnglishClass.Application.Models;

public record UserModel
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string PostCode { get; init; } = string.Empty;
    public string MobilePhone { get; init; } = string.Empty;
    public DateTime? CreationTime { get; init; }
}
