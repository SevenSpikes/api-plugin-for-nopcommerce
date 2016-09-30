using AutoMapper;
using Nop.Admin.Extensions;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ShoppingCartItemDtoMappings
    {
        public static ShoppingCartItemDto ToDto(this ShoppingCartItem shoppingCartItem)
        {
            return shoppingCartItem.MapTo<ShoppingCartItem, ShoppingCartItemDto>();
        }
    }
}