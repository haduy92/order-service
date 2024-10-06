using AutoMapper;
using FlashCard.Application.Models;
using FlashCard.Domain.Entities;
using FlashCard.Infrastructure.Models;

namespace FlashCard.Infrastructure.Mapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ReverseMap();
        CreateMap<Card, CardDto>()
            .ReverseMap();
        CreateMap<CreateCardRequest, Card>();
    }
}
