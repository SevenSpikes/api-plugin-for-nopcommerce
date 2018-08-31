using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.ShoppingCarts
{
    public class ShoppingCartItemsRootObject
    {
        public ShoppingCartItemsRootObject()
        {
            ShoppingCartItems = new List<ShoppingCartItemDto>();
        }

        [JsonProperty("shopping_carts")]
        public IList<ShoppingCartItemDto> ShoppingCartItems { get; set; }

    }
}