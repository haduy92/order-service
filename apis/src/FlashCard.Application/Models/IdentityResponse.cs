namespace FlashCard.Application.Models;

public record IdentityResponse : ResponseBase
{
    public required string UserId { get; init; }

    /// <summary>
    /// Gets or sets the authentication token for the user.
    /// This token is usually used for subsequent requests to the server to verify the user's identity and permissions.
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Gets or sets the expiration time for the authentication token.
    /// This indicates the time until which the token is valid. After this time, the user may need to log in again.
    /// </summary>
    public DateTime Expire { get; init; }
}
