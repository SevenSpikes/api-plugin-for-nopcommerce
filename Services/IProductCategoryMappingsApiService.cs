using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using static Nop.Plugin.Api.Infrastructure.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface IProductCategoryMappingsApiService
    {
        IList<ProductCategory> GetMappings(int? productId = null, int? categoryId = null, int limit = Configurations.DefaultLimit, 
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        int GetMappingsCount(int? productId = null, int? categoryId = null);

        ProductCategory GetById(int id);
    }
}