using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Orders
{
    public class OrdersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}