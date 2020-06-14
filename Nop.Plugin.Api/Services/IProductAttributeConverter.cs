using System.Collections.Generic;
using Nop.Plugin.Api.DTO;

namespace Nop.Plugin.Api.Services
{
    public interface IProductAttributeConverter
    {
        List<ProductItemAttributeDto> Parse(string attributesXml);
        string ConvertToXml(List<ProductItemAttributeDto> attributeDtos, int productId);
    }
}
