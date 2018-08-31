using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}