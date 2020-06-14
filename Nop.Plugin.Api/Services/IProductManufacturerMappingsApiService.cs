using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Services
{
    public interface IProductManufacturerMappingsApiService
    {
        IList<ProductManufacturer> GetMappings(
            int? productId = null, int? manufacturerId = null, int limit = Constants.Configurations.DefaultLimit,
            int page = Constants.Configurations.DefaultPageValue, int sinceId = Constants.Configurations.DefaultSinceId);

        int GetMappingsCount(int? productId = null, int? manufacturerId = null);

        ProductManufacturer GetById(int id);
    }
}
