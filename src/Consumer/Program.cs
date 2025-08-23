using Consumer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configure logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

// Register services following dependency flow
builder.Services.AddConsumerConfiguration(builder.Configuration);
builder.Services.AddMessagingInfrastructure();
builder.Services.AddExternalApiServices(builder.Configuration);
builder.Services.AddConsumerServices();
builder.Services.AddMessageHandling();
builder.Services.AddConsumerHostedServices();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting Order Consumer Service...");

await app.RunAsync();
