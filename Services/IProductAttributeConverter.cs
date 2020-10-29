using Nop.Plugin.Api.DTO;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Services
{
    public interface IProductAttributeConverter
    {
        List<ProductItemAttributeDto> Parse(string attributesXml);
        string ConvertToXml(List<ProductItemAttributeDto> attributeDtos, int productId);
    }
}
