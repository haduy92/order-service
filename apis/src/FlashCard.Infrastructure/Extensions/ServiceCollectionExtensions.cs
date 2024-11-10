using AutoMapper;
using FlashCard.Application.Interfaces.Application;
using FlashCard.Application.Interfaces.Identity;
using FlashCard.Application.Interfaces.Persistence.Cards;
using FlashCard.Application.Interfaces.Persistence.Identities;
using FlashCard.Infrastructure.Configurations;
using FlashCard.Infrastructure.Data;
using FlashCard.Infrastructure.Mapper;
using FlashCard.Infrastructure.Models;
using FlashCard.Infrastructure.Repositories;
using FlashCard.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlashCard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<OpenAIOptions>(configuration.GetSection(OpenAIOptions.SectionName));

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
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Add services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITextGeneratingService, TextGeneratingService>();

        // Add mapper profile
        MapperConfiguration mappingConfig = new(mc =>
        {
            mc.AddProfile(new InfrastructureProfile());
        });
        services.AddSingleton(mappingConfig.CreateMapper());

        return services;
    }
}
