using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.ProductAttributes
{
    [JsonObject(Title = "product_attribute")]
    public class ProductAttributeDto
    {
        /// <summary>
        /// Gets or sets the product attribute id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}