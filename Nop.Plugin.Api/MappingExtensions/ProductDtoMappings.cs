using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductDtoMappings
    {
        public static ProductDto ToDto(this Product product)
        {
            return Mapper.Map<Product, ProductDto>(product);
        }
    }
}