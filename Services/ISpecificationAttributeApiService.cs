using Nop.Core.Domain.Catalog;
using static Nop.Plugin.Api.Infrastructure.Constants;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Services
{
    public interface ISpecificationAttributeApiService
    {
        IList<ProductSpecificationAttribute> GetProductSpecificationAttributes(int? productId = null, int? specificationAttributeOptionId = null, bool? allowFiltering = null, bool? showOnProductPage = null, int limit = Configurations.DefaultLimit,  int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);
        IList<SpecificationAttribute> GetSpecificationAttributes(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);
    }
}