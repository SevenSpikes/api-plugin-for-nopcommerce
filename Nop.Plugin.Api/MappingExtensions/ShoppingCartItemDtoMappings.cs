using AutoMapper;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ShoppingCartItemDtoMappings
    {
        public static ShoppingCartItemDto ToDto(this ShoppingCartItem shoppingCartItem)
        {
            return Mapper.DynamicMap<ShoppingCartItem, ShoppingCartItemDto>(shoppingCartItem);
        }
    }
}