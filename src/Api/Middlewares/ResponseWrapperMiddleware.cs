using Application.Models;
using System.Text.Json;

namespace Api.Middlewares;

/// <summary>
/// Middleware that wraps all API responses in ResponseBase{T}
/// </summary>
public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrapperMiddleware> _logger;

    public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only wrap responses for API endpoints
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Only wrap successful responses (2xx status codes)
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

                if (!string.IsNullOrEmpty(responseContent))
                {
                    // Try to parse the response to see if it's already wrapped
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(responseContent);
                        var root = jsonDoc.RootElement;

                        // Check if it already has the ResponseBase structure
                        if (root.ValueKind == JsonValueKind.Object && 
                            (root.TryGetProperty("succeeded", out _) || root.TryGetProperty("errors", out _)))
                        {
                            // Already wrapped, just copy the response
                            await CopyResponseAsync(responseContent, originalBodyStream, context);
                            return;
                        }

                        // Parse the response as object to preserve its structure
                        var responseData = JsonSerializer.Deserialize<object>(responseContent);
                        
                        // Wrap the response
                        var wrappedResponse = new SuccessResponse<object>
                        {
                            Data = responseData
                        };

                        var wrappedJson = JsonSerializer.Serialize(wrappedResponse, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                        });

                        context.Response.ContentType = "application/json";
                        await CopyResponseAsync(wrappedJson, originalBodyStream, context);
                    }
                    catch (JsonException)
                    {
                        // If it's not JSON, just copy as is
                        await CopyResponseAsync(responseContent, originalBodyStream, context);
                    }
                }
                else
                {
                    // Empty response, wrap it
                    var wrappedResponse = new SuccessResponse();
                    var wrappedJson = JsonSerializer.Serialize(wrappedResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    });

                    context.Response.ContentType = "application/json";
                    await CopyResponseAsync(wrappedJson, originalBodyStream, context);
                }
            }
            else
            {
                // Error responses, copy as is (FastEndpoints handles error formatting)
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static async Task CopyResponseAsync(string content, Stream destination, HttpContext context)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        context.Response.ContentLength = bytes.Length;
        await destination.WriteAsync(bytes);
    }
}
