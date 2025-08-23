namespace Consumer.Services;

/// <summary>
/// Service for managing system authentication tokens
/// Follows the Single Responsibility Principle by handling only token management
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
