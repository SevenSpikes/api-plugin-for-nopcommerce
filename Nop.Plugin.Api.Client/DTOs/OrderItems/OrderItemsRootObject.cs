using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.OrderItems
{
    public class OrderItemsRootObject
    {
        public OrderItemsRootObject()
        {
            OrderItems = new List<OrderItemDto>();
        }

        [JsonProperty("order_items")]
        public IList<OrderItemDto> OrderItems { get; set; }

    }
}