using Application.Models.Order;
using Application.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Domain;
using System.Text;

namespace Consumer.Services;

public interface IOrderApiService
{
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    Task<OrderDetailsDto?> GetOrderAsync(int orderId);
}

/// <summary>
/// Service for interacting with the Order API
/// Uses EnsureSuccessStatusCode() for automatic error handling via delegating handlers
/// Follows the Single Responsibility Principle by focusing only on API communication logic
/// </summary>
public class OrderApiService : IOrderApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderApiService> _logger;

    public OrderApiService(HttpClient httpClient, ILogger<OrderApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        try
        {
            var request = new { Status = newStatus };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/v1/orders/{orderId}/status", content);
            
            // Use EnsureSuccessStatusCode - error handling is done by the delegating handler
            response.EnsureSuccessStatusCode();
            
            _logger.LogInformation("Successfully updated order {OrderId} status to {Status}", orderId, newStatus);
            return true;
        }
        catch (HttpRequestException ex)
        {
            // HTTP errors are already logged by the delegating handler
            // Just log the business context and return false
            _logger.LogWarning("Order status update failed for order {OrderId} to status {Status}: {Message}", 
                orderId, newStatus, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            // Handle non-HTTP exceptions (network issues, serialization errors, etc.)
            _logger.LogError(ex, "Unexpected error updating order {OrderId} status to {Status}", orderId, newStatus);
            return false;
        }
    }

    public async Task<OrderDetailsDto?> GetOrderAsync(int orderId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/orders/{orderId}");
            
            // Use EnsureSuccessStatusCode - error handling is done by the delegating handler
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Received response for order {OrderId}: {Response}", orderId, json);
            
            // Try to deserialize as wrapped response first
            try
            {
                var wrappedResponse = JsonConvert.DeserializeObject<SuccessResponse<OrderDetailsDto>>(json);
                if (wrappedResponse?.Succeeded == true && wrappedResponse.Data != null)
                {
                    _logger.LogInformation("Successfully retrieved order {OrderId} from API", orderId);
                    return wrappedResponse.Data;
                }
                else
                {
                    _logger.LogWarning("API returned unsuccessful response for order {OrderId}: {Errors}", 
                        orderId, wrappedResponse?.Errors != null ? string.Join(", ", wrappedResponse.Errors) : "Unknown error");
                    return null;
                }
            }
            catch (JsonException)
            {
                // Fallback: try to deserialize as direct OrderDetailsDto (in case response isn't wrapped)
                var orderDetails = JsonConvert.DeserializeObject<OrderDetailsDto>(json);
                if (orderDetails != null)
                {
                    _logger.LogInformation("Successfully retrieved order {OrderId} from API (direct response)", orderId);
                    return orderDetails;
                }
            }
            
            _logger.LogWarning("Failed to deserialize order response for {OrderId}", orderId);
            return null;
        }
        catch (HttpRequestException ex)
        {
            // HTTP errors are already logged by the delegating handler
            // Just log the business context and return null
            _logger.LogWarning("Order retrieval failed for order {OrderId}: {Message}", orderId, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            // Handle non-HTTP exceptions (network issues, serialization errors, etc.)
            _logger.LogError(ex, "Unexpected error getting order {OrderId}", orderId);
            return null;
        }
    }
}
