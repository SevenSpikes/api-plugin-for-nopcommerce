using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Categories
{
    public class CategoriesRootObject
    {
        public CategoriesRootObject()
        {
            Categories = new List<CategoryDto>();
        }

        [JsonProperty("categories")]
        public IList<CategoryDto> Categories { get; set; }

    }
}