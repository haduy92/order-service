using Application.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Shared.Extensions;

namespace Consumer.Services;

/// <summary>
/// HTTP delegating handler that automatically handles API error responses with detailed logging
/// Follows the Decorator pattern and Single Responsibility Principle
/// Provides centralized error handling for all HTTP requests
/// </summary>
public class ApiErrorHandlingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<ApiErrorHandlingDelegatingHandler> _logger;

    public ApiErrorHandlingDelegatingHandler(ILogger<ApiErrorHandlingDelegatingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        // If the response is not successful, log the error details before returning
        if (!response.IsSuccessStatusCode)
        {
            await LogApiErrorAsync(request, response);
        }

        return response;
    }

    /// <summary>
    /// Logs detailed API error information including request context and response details
    /// </summary>
    /// <param name="request">The HTTP request that resulted in an error</param>
    /// <param name="response">The HTTP response containing the error</param>
    private async Task LogApiErrorAsync(HttpRequestMessage request, HttpResponseMessage response)
    {
        var statusCode = response.StatusCode;
        var reasonPhrase = response.ReasonPhrase ?? "Unknown";
        var method = request.Method.ToString();
        var requestUri = request.RequestUri?.ToString() ?? "Unknown";
        
        string? responseBody = null;
        string? errorDetails = null;

        try
        {
            // Read the response body to get detailed error information
            responseBody = await response.Content.ReadAsStringAsync();
            
            if (!responseBody.IsNullOrWhiteSpace())
            {
                // Try to parse as structured error response
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBody);
                    if (errorResponse?.Errors != null && errorResponse.Errors.Any())
                    {
                        errorDetails = string.Join("; ", errorResponse.Errors);
                    }
                }
                catch (JsonException)
                {
                    // If not a structured error response, use raw response body (truncate if too long)
                    errorDetails = responseBody.Length > 500 
                        ? $"{responseBody.Left(500)}... (truncated)"
                        : responseBody;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read error response body for {Method} {RequestUri}", method, requestUri);
        }

        // Build comprehensive log message
        var logMessage = "API request failed: {Method} {RequestUri} - Status: {StatusCode} ({ReasonPhrase})";
        var logArgs = new List<object> { method, requestUri, statusCode, reasonPhrase };

        if (!errorDetails.IsNullOrWhiteSpace())
        {
            logMessage += " - Error Details: {ErrorDetails}";
            logArgs.Add(errorDetails!);
        }

        // Determine appropriate log level based on HTTP status code
        var logLevel = DetermineLogLevel(statusCode);
        _logger.Log(logLevel, logMessage, logArgs.ToArray());

        // Log request and response headers for diagnostic purposes (debug level only)
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LogRequestResponseHeaders(request, response, method, requestUri, statusCode);
        }
    }

    /// <summary>
    /// Determines the appropriate log level based on HTTP status code
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <returns>The appropriate log level</returns>
    private static LogLevel DetermineLogLevel(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.NotFound => LogLevel.Information, // 404 might be expected in some scenarios
        HttpStatusCode.Unauthorized or 
        HttpStatusCode.Forbidden => LogLevel.Warning, // Authentication/authorization issues
        HttpStatusCode.BadRequest or
        HttpStatusCode.UnprocessableEntity or
        HttpStatusCode.Conflict => LogLevel.Warning, // Client validation errors
        HttpStatusCode.TooManyRequests => LogLevel.Warning, // Rate limiting
        _ when ((int)statusCode >= 500) => LogLevel.Error, // Server errors are critical
        _ => LogLevel.Warning // Other client errors
    };

    /// <summary>
    /// Logs HTTP request and response headers for diagnostic purposes
    /// </summary>
    /// <param name="request">The HTTP request</param>
    /// <param name="response">The HTTP response</param>
    /// <param name="method">The HTTP method</param>
    /// <param name="requestUri">The request URI</param>
    /// <param name="statusCode">The HTTP status code</param>
    private void LogRequestResponseHeaders(
        HttpRequestMessage request, 
        HttpResponseMessage response, 
        string method, 
        string requestUri, 
        HttpStatusCode statusCode)
    {
        try
        {
            var requestHeaders = request.Headers
                .Concat(request.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                .ToDictionary(h => h.Key, h => string.Join(", ", h.Value));
            
            var responseHeaders = response.Headers
                .Concat(response.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                .ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

            _logger.LogDebug("Request/Response headers for {Method} {RequestUri} ({StatusCode}):\nRequest Headers: {RequestHeaders}\nResponse Headers: {ResponseHeaders}", 
                method, requestUri, statusCode, 
                JsonConvert.SerializeObject(requestHeaders, Formatting.Indented),
                JsonConvert.SerializeObject(responseHeaders, Formatting.Indented));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log request/response headers for {Method} {RequestUri}", method, requestUri);
        }
    }
}
