using FastEndpoints;
using Application.Contracts.Identity;
using Application.Models.Auth;
using Microsoft.Extensions.Configuration;
using Shared.Extensions;

namespace Api.Endpoints.Auth;

/// <summary>
/// System authentication endpoint for service-to-service communication
/// Only accessible with proper system credentials
/// </summary>
public class SystemTokenEndpoint : Ep
    .Req<SystemTokenEndpoint.RequestDto>
    .Res<SystemTokenResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public SystemTokenEndpoint(ITokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Post("system-token");
        Group<AuthGroup>();
        AllowAnonymous(); // We handle authentication manually for system accounts
        Description(bld => bld.WithName("Auth_SystemToken")
            .Produces<SystemTokenResponse>(statusCode: StatusCodes.Status200OK)
            .Produces(statusCode: StatusCodes.Status401Unauthorized));
        Summary(s =>
        {
            s.Summary = "Generate system token for service-to-service authentication.";
            s.Description = "An endpoint for generating system tokens for internal services.";
        });
    }

    public override async Task<SystemTokenResponse> ExecuteAsync(RequestDto req, CancellationToken cancellationToken)
    {
        // Get system secret from configuration
        var systemSecret = _configuration["SystemAuth:Secret"] ?? "your-system-secret-key";
        
        // Validate system credentials using case-insensitive comparison for better security
        if (!req.SystemSecret.EqualsIgnoreCase(systemSecret))
        {
            ThrowError("Invalid system credentials", StatusCodes.Status401Unauthorized);
        }

        // Get system user details from configuration
        var systemUserId = _configuration["SystemUser:Id"] ?? "system-user";
        var systemUserEmail = _configuration["SystemUser:Email"] ?? "system@yourcompany.com";

        // Generate system token using the base token service
        var systemToken = _tokenService.GenerateAccessToken(systemUserId, systemUserEmail);
        
        return new SystemTokenResponse
        {
            AccessToken = systemToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60) // System tokens can have longer expiry
        };
    }

    public sealed record RequestDto
    {
        public required string SystemSecret { get; init; }
        public string? ServiceName { get; init; }
    }
}

public record SystemTokenResponse
{
    public required string AccessToken { get; init; }
    public DateTime ExpiresAt { get; init; }
}
