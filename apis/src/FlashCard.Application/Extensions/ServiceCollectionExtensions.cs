using AutoMapper;
using FlashCard.Application.Mapper;
using Microsoft.Extensions.DependencyInjection;

namespace FlashCard.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
        {
            MapperConfiguration mappingConfig = new(mc =>
            {
                mc.AddProfile(new ApplicationProfile());
            });

            services.AddSingleton(mappingConfig.CreateMapper());
        }
}
