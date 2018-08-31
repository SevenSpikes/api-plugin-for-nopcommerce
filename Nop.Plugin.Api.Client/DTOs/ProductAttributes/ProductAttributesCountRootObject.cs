using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.ProductAttributes
{
    public class ProductAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}