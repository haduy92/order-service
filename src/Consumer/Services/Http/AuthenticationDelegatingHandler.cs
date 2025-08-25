using Consumer.Services.Infrastructure.ExternalApi;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Consumer.Services.Http;

/// <summary>
/// HTTP delegating handler that automatically adds authentication headers to API requests
/// Follows the Decorator pattern to extend HttpClient functionality
/// Part of Infrastructure layer - handles HTTP request authentication
/// </summary>
public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly ISystemTokenService _systemTokenService;
    private readonly ILogger<AuthenticationDelegatingHandler> _logger;

    public AuthenticationDelegatingHandler(
        ISystemTokenService systemTokenService,
        ILogger<AuthenticationDelegatingHandler> logger)
    {
        _systemTokenService = systemTokenService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        // Get system token and add it to the request
        var token = await _systemTokenService.GetSystemTokenAsync();
        
        if (!token.IsNullOrWhiteSpace())
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            _logger.LogDebug("Added authentication header to request: {Method} {Uri}", 
                request.Method, request.RequestUri);
        }
        else
        {
            _logger.LogWarning("No system token available for request: {Method} {Uri}", 
                request.Method, request.RequestUri);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If we get a 401, try to refresh the token and retry once
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !token.IsNullOrWhiteSpace())
        {
            _logger.LogInformation("Received 401 response, attempting to refresh system token");
            
            var newToken = await _systemTokenService.AuthenticateSystemUserAsync();
            if (!newToken.IsNullOrWhiteSpace())
            {
                // Update the authorization header and retry
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);
                _logger.LogDebug("Retrying request with new token: {Method} {Uri}", 
                    request.Method, request.RequestUri);
                
                response.Dispose(); // Dispose the failed response
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}