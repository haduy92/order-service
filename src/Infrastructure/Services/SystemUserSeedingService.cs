using Application.Contracts.Identity;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Infrastructure.Services;

/// <summary>
/// Service responsible for seeding system users and roles
/// Follows Single Responsibility Principle by handling only data seeding
/// </summary>
public interface ISystemUserSeedingService
{
    /// <summary>
    /// Seeds the system user account if it doesn't exist
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task SeedSystemUserAsync();
}

/// <summary>
/// Implementation of system user seeding service
/// Handles creation of system accounts for service-to-service authentication
/// </summary>
public class SystemUserSeedingService : ISystemUserSeedingService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SystemUserSeedingService> _logger;

    // System user constants
    private const string SystemUserEmail = "system@yourcompany.com";
    private const string SystemUserPassword = "SystemPassword123!";
    private const string SystemRoleName = "System";

    public SystemUserSeedingService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<SystemUserSeedingService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedSystemUserAsync()
    {
        try
        {
            // Create system role if it doesn't exist
            await CreateSystemRoleIfNotExistsAsync();

            // Create system user if it doesn't exist
            await CreateSystemUserIfNotExistsAsync();

            _logger.LogInformation("System user seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during system user seeding");
            throw;
        }
    }

    private async Task CreateSystemRoleIfNotExistsAsync()
    {
        var systemRoleExists = await _roleManager.RoleExistsAsync(SystemRoleName);
        if (!systemRoleExists)
        {
            var systemRole = new IdentityRole(SystemRoleName)
            {
                NormalizedName = SystemRoleName.ToUpperInvariant()
            };

            var result = await _roleManager.CreateAsync(systemRole);
            if (result.Succeeded)
            {
                _logger.LogInformation("System role '{SystemRole}' created successfully", SystemRoleName);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create system role '{SystemRole}': {Errors}", SystemRoleName, errors);
                throw new InvalidOperationException($"Failed to create system role: {errors}");
            }
        }
        else
        {
            _logger.LogDebug("System role '{SystemRole}' already exists", SystemRoleName);
        }
    }

    private async Task CreateSystemUserIfNotExistsAsync()
    {
        var systemUser = await _userManager.FindByEmailAsync(SystemUserEmail);
        if (systemUser == null)
        {
            // Get credentials from configuration or use defaults
            var email = _configuration["SystemUser:Email"] ?? SystemUserEmail;
            var password = _configuration["SystemUser:Password"] ?? SystemUserPassword;

            systemUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = email,
                Email = email,
                NormalizedUserName = email.ToUpperInvariant(),
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(systemUser, password);
            if (result.Succeeded)
            {
                _logger.LogInformation("System user '{SystemEmail}' created successfully", email);

                // Add user to system role
                var roleResult = await _userManager.AddToRoleAsync(systemUser, SystemRoleName);
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("System user '{SystemEmail}' added to '{SystemRole}' role", email, SystemRoleName);
                }
                else
                {
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to add system user to role: {Errors}", roleErrors);
                }
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create system user '{SystemEmail}': {Errors}", email, errors);
                throw new InvalidOperationException($"Failed to create system user: {errors}");
            }
        }
        else
        {
            _logger.LogDebug("System user '{SystemEmail}' already exists", SystemUserEmail);
            
            // Ensure system user is in the system role
            var isInRole = await _userManager.IsInRoleAsync(systemUser, SystemRoleName);
            if (!isInRole)
            {
                var roleResult = await _userManager.AddToRoleAsync(systemUser, SystemRoleName);
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Added existing system user to '{SystemRole}' role", SystemRoleName);
                }
            }
        }
    }
}
