namespace Consumer.Configuration;

public class OrderApiOptions
{
    public const string SectionName = "OrderApi";
    
    public string BaseUrl { get; set; } = "http://localhost:5000";
    
    /// <summary>
    /// System token for service-to-service authentication
    /// </summary>
    public string? SystemToken { get; set; }
    
    /// <summary>
    /// Alternative: System user credentials for generating tokens
    /// </summary>
    public SystemCredentials? SystemCredentials { get; set; }
}

public class SystemCredentials
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
