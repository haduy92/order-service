using Application.Models;
using Shared.Exceptions;
using System.Net;
using System.Text.Json;

namespace Api.Middlewares;

/// <summary>
/// Middleware that handles exceptions and returns appropriate error responses
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        ErrorResponse errorResponse;
        
        switch (exception)
        {
            case UnauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = new ErrorResponse(exception.Message);
                break;
                
            case DuplicatedException:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse = new ErrorResponse(exception.Message);
                break;
                
            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = new ErrorResponse(exception.Message);
                break;
                
            case ValidationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse(exception.Message);
                break;
                
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse("An internal server error occurred");
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await response.WriteAsync(jsonResponse);
    }
}
