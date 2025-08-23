using Consumer.Abstractions;
using Consumer.Configuration;
using Consumer.Contracts;
using Consumer.Factories;
using Consumer.MessageHandlers;
using Consumer.Services;
using Consumer.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Consumer.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageHandling(this IServiceCollection services)
    {
        // Register message handler registry and factory
        services.AddSingleton<IMessageHandlerRegistry, MessageHandlerRegistry>();
        services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
        
        // Register message handlers - they will be auto-discovered by the registry
        services.AddScoped<OrderCreatedMessageHandler>();
        services.AddScoped<OrderStatusChangedMessageHandler>();
        
        return services;
    }

    public static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services)
    {
        // Register RabbitMQ infrastructure services
        services.AddScoped<IRabbitMqConnectionService, RabbitMqConnectionService>();
        services.AddScoped<IQueueSetupService, QueueSetupService>();
        
        return services;
    }

    public static IServiceCollection AddConsumerConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Configure options from appsettings with validation
        services.Configure<OrderApiOptions>(configuration.GetSection(OrderApiOptions.SectionName));
        
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<IValidateOptions<RabbitMqOptions>, RabbitMqOptionsValidator>();
        
        return services;
    }

    public static IServiceCollection AddConsumerServices(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IOrderProcessingService, OrderProcessingService>();
        
        return services;
    }

    public static IServiceCollection AddConsumerHostedServices(this IServiceCollection services)
    {
        // Register background services
        services.AddHostedService<RabbitMqConsumerService>();
        
        return services;
    }

    public static IServiceCollection AddExternalApiServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Register system authentication services
        services.AddScoped<ISystemTokenService, SystemTokenService>();
        
        // Register HTTP delegating handlers
        services.AddTransient<AuthenticationDelegatingHandler>();
        services.AddTransient<ApiErrorHandlingDelegatingHandler>();
        
        // Register HTTP client for system token authentication (separate from main API client)
        services.AddHttpClient<ISystemTokenService, SystemTokenService>((serviceProvider, client) =>
        {
            var orderApiOptions = configuration.GetSection(OrderApiOptions.SectionName).Get<OrderApiOptions>();
            client.BaseAddress = new Uri(orderApiOptions?.BaseUrl ?? "http://localhost:5000");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<ApiErrorHandlingDelegatingHandler>(); // Add error handling
        
        // Register HTTP client for external API calls with authentication and error handling
        services.AddHttpClient<IOrderApiService, OrderApiService>((serviceProvider, client) =>
        {
            var orderApiOptions = configuration.GetSection(OrderApiOptions.SectionName).Get<OrderApiOptions>();
            client.BaseAddress = new Uri(orderApiOptions?.BaseUrl ?? "http://localhost:5000");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthenticationDelegatingHandler>() // Add authentication first
        .AddHttpMessageHandler<ApiErrorHandlingDelegatingHandler>(); // Add error handling last
        
        return services;
    }
}
