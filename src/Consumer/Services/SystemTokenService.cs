using Consumer.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using Shared.Extensions;

namespace Consumer.Services;

/// <summary>
/// Implementation of system token service for Consumer authentication
/// Handles both pre-configured tokens and dynamic authentication
/// Uses EnsureSuccessStatusCode() for automatic error handling via delegating handlers
/// </summary>
public class SystemTokenService : ISystemTokenService
{
    private readonly OrderApiOptions _orderApiOptions;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SystemTokenService> _logger;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public SystemTokenService(
        IOptions<OrderApiOptions> orderApiOptions,
        HttpClient httpClient,
        ILogger<SystemTokenService> logger)
    {
        _orderApiOptions = orderApiOptions.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string?> GetSystemTokenAsync()
    {
        // First, try to use pre-configured system token
        if (!_orderApiOptions.SystemToken.IsNullOrWhiteSpace())
        {
            _logger.LogDebug("Using pre-configured system token");
            return _orderApiOptions.SystemToken;
        }

        // If cached token is still valid, use it
        if (!_cachedToken.IsNullOrWhiteSpace() && DateTime.UtcNow < _tokenExpiry)
        {
            _logger.LogDebug("Using cached system token");
            return _cachedToken;
        }

        // Otherwise, authenticate and get a new token
        _logger.LogInformation("Obtaining new system token through authentication");
        return await AuthenticateSystemUserAsync();
    }

    public async Task<string?> AuthenticateSystemUserAsync()
    {
        if (_orderApiOptions.SystemCredentials == null ||
            _orderApiOptions.SystemCredentials.Email.IsNullOrWhiteSpace() ||
            _orderApiOptions.SystemCredentials.Password.IsNullOrWhiteSpace())
        {
            _logger.LogError("System credentials are not configured properly");
            return null;
        }

        try
        {
            var loginRequest = new
            {
                Email = _orderApiOptions.SystemCredentials.Email,
                Password = _orderApiOptions.SystemCredentials.Password
            };

            var json = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/auth/signin", content);

            // Use EnsureSuccessStatusCode - error handling is done by the delegating handler
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(responseJson);

            if (authResponse?.Succeeded == true && authResponse.Data != null && !authResponse.Data.AccessToken.IsNullOrWhiteSpace())
            {
                _cachedToken = authResponse.Data.AccessToken;
                // Set token expiry to 80% of the actual expiry to ensure refresh before expiration
                _tokenExpiry = DateTime.UtcNow.AddMinutes(12); // Assuming 15-minute tokens, refresh at 12 minutes
                
                _logger.LogInformation("Successfully obtained system token");
                return _cachedToken;
            }
            else
            {
                _logger.LogError("Authentication succeeded but response format is invalid: {Errors}", 
                    authResponse?.Errors != null ? string.Join(", ", authResponse.Errors) : "Unknown error");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            // HTTP errors are already logged by the delegating handler
            // Just log the business context
            _logger.LogWarning("System user authentication failed: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            // Handle non-HTTP exceptions (network issues, serialization errors, etc.)
            _logger.LogError(ex, "Unexpected error during system user authentication");
            return null;
        }
    }

    // Response DTOs for authentication
    private class AuthenticationResponse
    {
        public bool Succeeded { get; set; }
        public AuthenticationData? Data { get; set; }
        public string[]? Errors { get; set; }
    }

    private class AuthenticationData
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserId { get; set; }
    }
}
