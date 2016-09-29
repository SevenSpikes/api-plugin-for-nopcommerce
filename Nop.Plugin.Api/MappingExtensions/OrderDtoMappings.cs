using AutoMapper;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Orders;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class OrderDtoMappings
    {
        public static OrderDto ToDto(this Order order)
        {
            return Mapper.DynamicMap<Order, OrderDto>(order);
        }
    }
}