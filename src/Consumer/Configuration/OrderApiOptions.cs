namespace Consumer.Configuration;

public class OrderApiOptions
{
    public const string SectionName = "OrderApi";
    
    public string BaseUrl { get; set; } = "http://localhost:5000";
}
