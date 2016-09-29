using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Customers;

namespace Nop.Plugin.Api.DTOs.Products
{
    public class ProductsRootObjectDto : ISerializableObject
    {
        public ProductsRootObjectDto()
        {
            Products = new List<ProductDto>();
        }

        [JsonProperty("products")]
        public IList<ProductDto> Products { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "products";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ProductDto);
        }
    }
}