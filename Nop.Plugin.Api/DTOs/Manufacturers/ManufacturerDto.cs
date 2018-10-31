using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Manufacturers
{
    [JsonObject(Title = "manufacturer")]
    public class ManufacturerDto : BaseDto
    {
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

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        [JsonProperty("published")]
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a display order.
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }
    }
}
