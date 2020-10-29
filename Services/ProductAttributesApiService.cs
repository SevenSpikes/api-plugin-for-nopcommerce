using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Api.DataStructures;
using System.Collections.Generic;
using System.Linq;
using static Nop.Plugin.Api.Infrastructure.Constants;

namespace Nop.Plugin.Api.Services
{
    public class ProductAttributesApiService : IProductAttributesApiService
    {
        private readonly IRepository<ProductAttribute> _productAttributesRepository;

        public ProductAttributesApiService(IRepository<ProductAttribute> productAttributesRepository)
        {
            _productAttributesRepository = productAttributesRepository;
        }

        public IList<ProductAttribute> GetProductAttributes(int limit = Configurations.DefaultLimit,
             int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetProductAttributesQuery(sinceId);

            return new ApiList<ProductAttribute>(query, page - 1, limit);
        }

        public int GetProductAttributesCount()
        {
            return GetProductAttributesQuery().Count();
        }

        ProductAttribute IProductAttributesApiService.GetById(int id)
        {
            if (id <= 0)
                return null;

            return _productAttributesRepository.GetById(id);
        }

        private IQueryable<ProductAttribute> GetProductAttributesQuery(int sinceId = Configurations.DefaultSinceId)
        {
            var query = _productAttributesRepository.Table;

            if (sinceId > 0)
            {
                query = query.Where(productAttribute => productAttribute.Id > sinceId);
            }

            query = query.OrderBy(productAttribute => productAttribute.Id);

            return query;
        }
    }
}