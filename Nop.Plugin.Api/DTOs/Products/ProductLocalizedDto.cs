using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Products
{
    public class ProductLocalizedDto
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        [JsonProperty("language_id")]
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the localized name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        [JsonProperty("short_description")]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        [JsonProperty("full_description")]
        public string FullDescription { get; set; }
    }
}
