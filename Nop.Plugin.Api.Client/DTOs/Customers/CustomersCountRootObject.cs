using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Customers
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; } 
    }
}