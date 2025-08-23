using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

/// <summary>
/// Extension methods for seeding and initializing the database
/// Follows the Single Responsibility Principle by handling only database initialization
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Seeds the database with initial data including system users
    /// Should be called during application startup
    /// </summary>
    /// <param name="host">The host to get services from</param>
    /// <returns>Task representing the async operation</returns>
    public static async Task SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<IHost>>();

        try
        {
            logger.LogInformation("Starting database seeding...");

            // Ensure databases are created and migrations are applied
            var identityContext = services.GetRequiredService<AppIdentityDbContext>();
            var appContext = services.GetRequiredService<AppDbContext>();

            await identityContext.Database.MigrateAsync();
            await appContext.Database.MigrateAsync();

            // Seed system user and roles
            var systemUserSeedingService = services.GetRequiredService<ISystemUserSeedingService>();
            await systemUserSeedingService.SeedSystemUserAsync();

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    /// <summary>
    /// Ensures database is created and migrations are applied
    /// Alternative to SeedDatabaseAsync when you only need migrations without seeding
    /// </summary>
    /// <param name="host">The host to get services from</param>
    /// <returns>Task representing the async operation</returns>
    public static async Task EnsureDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<IHost>>();

        try
        {
            logger.LogInformation("Ensuring database is created and up to date...");

            var identityContext = services.GetRequiredService<AppIdentityDbContext>();
            var appContext = services.GetRequiredService<AppDbContext>();

            await identityContext.Database.MigrateAsync();
            await appContext.Database.MigrateAsync();

            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while ensuring database");
            throw;
        }
    }
}
