using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Orders
{
    public class SingleOrderRootObject
    {
        [JsonProperty("order")]
        public OrderDto Order { get; set; }
    }
}