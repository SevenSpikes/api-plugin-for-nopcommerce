using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.ProductAttributes
{
    public class ProductAttributesRootObjectDto
    {
        public ProductAttributesRootObjectDto()
        {
            ProductAttributes = new List<ProductAttributeDto>();
        }

        [JsonProperty("product_attributes")]
        public IList<ProductAttributeDto> ProductAttributes { get; set; }
        
    }
}