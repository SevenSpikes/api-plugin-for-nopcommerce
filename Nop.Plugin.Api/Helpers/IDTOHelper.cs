using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Helpers
{
    public interface IDTOHelper
    {
        ProductDto PrepareProductDTO(Product product);
        CategoryDto PrepareCategoryDTO(Category category);
    }
}
