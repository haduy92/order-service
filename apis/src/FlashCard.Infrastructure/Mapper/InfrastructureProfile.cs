using AutoMapper;
using FlashCard.Application.Models;
using FlashCard.Infrastructure.Models;

namespace FlashCard.Infrastructure.Mapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ReverseMap();
    }
}
