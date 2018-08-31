using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.ProductCategoryMappings
{
    public class ProductCategoryMappingsRootObject
    {
        public ProductCategoryMappingsRootObject()
        {
            ProductCategoryMappingDtos = new List<ProductCategoryMappingDto>();
        }

        [JsonProperty("product_category_mappings")]
        public IList<ProductCategoryMappingDto> ProductCategoryMappingDtos { get; set; }
        
    }
}