using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.ProductCategoryMappings
{
    public class ProductCategoryMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}