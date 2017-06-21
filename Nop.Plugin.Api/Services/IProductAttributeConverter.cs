using Nop.Plugin.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Services
{
    public interface IProductAttributeConverter
    {
        List<ProductItemAttributeDto> Parse(string attributesXml);
        string ConvertToXml(List<ProductItemAttributeDto> attributeDtos, int productId);
    }
}
