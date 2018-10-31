using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Tax
{
    [JsonObject(Title = "tax_category")]
    public class TaxCategoryDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the tax category name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a display order.
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }
    }
}
