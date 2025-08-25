namespace Consumer.Services.Infrastructure.ExternalApi;

/// <summary>
/// Interface for system token authentication service
/// Part of Infrastructure layer - handles authentication with external APIs
/// </summary>
public interface ISystemTokenService
{
    /// <summary>
    /// Get a valid system token for API authentication
    /// </summary>
    /// <returns>Valid system token or null if unable to obtain one</returns>
    Task<string?> GetSystemTokenAsync();
    
    /// <summary>
    /// Authenticate using system credentials and get a new token
    /// </summary>
    /// <returns>New system token or null if authentication failed</returns>
    Task<string?> AuthenticateSystemUserAsync();
}
