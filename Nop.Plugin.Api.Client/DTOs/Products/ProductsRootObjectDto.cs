using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Products
{
    public class ProductsRootObjectDto
    {
        public ProductsRootObjectDto()
        {
            Products = new List<ProductDto>();
        }

        [JsonProperty("products")]
        public IList<ProductDto> Products { get; set; }
        
    }
}