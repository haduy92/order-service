using AutoMapper;
using FlashCard.Application.Interfaces;
using FlashCard.Application.Interfaces.Identity;
using FlashCard.Infrastructure.Data;
using FlashCard.Infrastructure.Mapper;
using FlashCard.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlashCard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
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
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        // Add services
        services.AddScoped<IAuthService, AuthService>();

        // Add mapper profile
        MapperConfiguration mappingConfig = new(mc =>
        {
            mc.AddProfile(new InfrastructureProfile());
        });
        services.AddSingleton(mappingConfig.CreateMapper());

        return services;
    }
}
