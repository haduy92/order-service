using Application.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Shared.Extensions;

namespace Consumer.Extensions;

/// <summary>
/// Extension methods for HTTP response logging and error handling
/// Follows the DRY principle by providing reusable HTTP response handling functionality
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Logs API error responses with detailed information including response body and headers
    /// </summary>
    /// <param name="response">The HTTP response that contains the error</param>
    /// <param name="logger">The logger instance</param>
    /// <param name="messageTemplate">The log message template</param>
    /// <param name="args">Arguments for the log message template</param>
    /// <returns>Task representing the async logging operation</returns>
    public static async Task LogApiErrorAsync(
        this HttpResponseMessage response, 
        ILogger logger, 
        string messageTemplate, 
        params object[] args)
    {
        var statusCode = response.StatusCode;
        var reasonPhrase = response.ReasonPhrase ?? "Unknown";
        string? responseBody = null;
        string? errorDetails = null;

        try
        {
            // Read the response body to get detailed error information
            responseBody = await response.Content.ReadAsStringAsync();
            
            if (!responseBody.IsNullOrWhiteSpace())
            {
                // Try to parse as error response to extract structured error information
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
            logger.LogWarning(ex, "Failed to read error response body for status code {StatusCode}", statusCode);
        }

        // Build enhanced log message with status and error details
        var enhancedArgs = new List<object>(args)
        {
            statusCode,
            reasonPhrase
        };

        var enhancedMessage = $"{messageTemplate}. Status: {{StatusCode}} ({{ReasonPhrase}})";

        if (!errorDetails.IsNullOrWhiteSpace())
        {
            enhancedArgs.Add(errorDetails!);
            enhancedMessage += ". Error Details: {ErrorDetails}";
        }

        // Determine appropriate log level based on HTTP status code
        var logLevel = DetermineLogLevel(statusCode);

        logger.Log(logLevel, enhancedMessage, enhancedArgs.ToArray());

        // Log response headers for diagnostic purposes (debug level only)
        if (logger.IsEnabled(LogLevel.Debug))
        {
            LogResponseHeaders(response, logger, statusCode);
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
        _ when ((int)statusCode >= 500) => LogLevel.Error, // Server errors are critical
        _ => LogLevel.Warning // Other client errors
    };

    /// <summary>
    /// Logs HTTP response headers for diagnostic purposes
    /// </summary>
    /// <param name="response">The HTTP response</param>
    /// <param name="logger">The logger instance</param>
    /// <param name="statusCode">The HTTP status code</param>
    private static void LogResponseHeaders(HttpResponseMessage response, ILogger logger, HttpStatusCode statusCode)
    {
        try
        {
            var headers = response.Headers
                .Concat(response.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                .ToDictionary(h => h.Key, h => string.Join(", ", h.Value));
            
            logger.LogDebug("Response headers for {StatusCode} response: {Headers}", 
                statusCode, JsonConvert.SerializeObject(headers, Formatting.Indented));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to log response headers for status code {StatusCode}", statusCode);
        }
    }
}
