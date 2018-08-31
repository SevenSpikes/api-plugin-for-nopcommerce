using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Orders
{
    public class OrdersRootObject
    {
        public OrdersRootObject()
        {
            Orders = new List<OrderDto>();
        }

        [JsonProperty("orders")]
        public List<OrderDto> Orders { get; set; }
        
    }
}