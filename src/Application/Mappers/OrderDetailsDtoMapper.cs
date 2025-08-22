using Application.Models.Order;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

[Mapper]
public partial class OrderDetailsDtoMapper
{
    public static partial OrderDetailsDto Map(Order order);
}

