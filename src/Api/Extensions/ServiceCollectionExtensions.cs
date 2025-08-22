using System.Text;
using Asp.Versioning;
using Api.Contexts;
using Application.Interfaces.Application;
using Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Repositories;
using Infrastructure.Configurations;
using FastEndpoints.Swagger;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDIs(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        return services;
    }

    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        // Configure main API document
        services.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.DocumentName = "v1";
                s.Title = "Order Service API";
                s.Version = "v1.0";
                s.Description = "A comprehensive API for managing orders and authentication";
            };
            
            o.EnableJWTBearerAuth = true;
            
            // Exclude endpoints that don't belong to this document
            o.ExcludeNonFastEndpoints = true;
            
            // Disable automatic tag generation to prevent duplicates
            o.AutoTagPathSegmentIndex = 0;
            
            // Configure JWT security scheme
            o.EndpointFilter = (endpointDefinition) =>
            {
                // Include all FastEndpoints
                return true;
            };
        });

        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions!.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                };
            });
        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddApiVersion(this IServiceCollection services)
    {
        // Minimal API versioning for future extensibility
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            // Use only header-based versioning to avoid URL conflicts
            options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
        });

        return services;
    }
}

