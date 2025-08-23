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

            var response = await _httpClient.PutAsync($"/api/orders/{orderId}/status", content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully updated order {OrderId} status to {Status}", orderId, newStatus);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to update order {OrderId} status. Response: {StatusCode}", orderId, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId} status to {Status}", orderId, newStatus);
            return false;
        }
    }

    public async Task<OrderDetailsDto?> GetOrderAsync(int orderId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/orders/{orderId}");
            
            if (response.IsSuccessStatusCode)
            {
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
            else
            {
                _logger.LogWarning("Failed to get order {OrderId}. Response: {StatusCode}", orderId, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", orderId);
            return null;
        }
    }
}
