using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Categories
{
    public class CategoriesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}