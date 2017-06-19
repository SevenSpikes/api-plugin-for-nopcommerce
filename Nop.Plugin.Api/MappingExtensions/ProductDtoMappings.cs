using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductDtoMappings
    {
        public static ProductDto ToDto(this Product product)
        {
            return Mapper.DynamicMap<Product, ProductDto>(product);
        }

        public static ProductAttributeValueDto ToDto(this ProductAttributeValue productAttributeValue)
        {
            return Mapper.DynamicMap<ProductAttributeValue, ProductAttributeValueDto>(productAttributeValue);
        }
    }
}