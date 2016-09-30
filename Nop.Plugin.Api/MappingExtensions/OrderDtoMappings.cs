using AutoMapper;
using Nop.Admin.Extensions;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Orders;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class OrderDtoMappings
    {
        public static OrderDto ToDto(this Order order)
        {
            return order.MapTo<Order, OrderDto>();
        }
    }
}