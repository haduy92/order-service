using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Contracts.Application;
using Application.Contracts.Identity;
using Application.Contracts.Messaging;
using Application.Contracts.Persistence;
using Application.Contracts.Persistence.Identities;
using Infrastructure.Configurations;
using Infrastructure.Data;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<OpenAIOptions>(configuration.GetSection(OpenAIOptions.SectionName));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        // Add IdentityDbContext
        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppIdentityDbContext).Assembly.FullName)));

        // Add AddDbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Add identity auth
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        // Add repositories
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        // Add services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITextGeneratingService, TextGeneratingService>();
        services.AddScoped<ISystemUserSeedingService, SystemUserSeedingService>();
        
        // Add messaging
        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();

        return services;
    }
}

