using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using static Nop.Plugin.Api.Infrastructure.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface IProductAttributesApiService
    {
        IList<ProductAttribute> GetProductAttributes(int limit = Configurations.DefaultLimit, 
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        int GetProductAttributesCount();

        ProductAttribute GetById(int id);
    }
}