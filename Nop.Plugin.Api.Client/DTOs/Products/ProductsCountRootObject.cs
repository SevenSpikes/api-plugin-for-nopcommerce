using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Products
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}