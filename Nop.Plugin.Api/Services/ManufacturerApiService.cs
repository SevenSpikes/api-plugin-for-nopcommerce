using System;
using System.Collections.Generic;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using System.Linq;
using System.Linq.Dynamic.Core;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Core.Domain.Messages;
using Nop.Core.Caching;
using Nop.Plugin.Api.Extensions;

namespace Nop.Plugin.Api.Services
{
    public class ManufacturerApiService : IManufacturerApiService
    {
        private readonly IStoreContext _storeContext;
        private readonly ILanguageService _languageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;
        private readonly IStaticCacheManager _cacheManager;

        public ManufacturerApiService(IRepository<Manufacturer> manufacturerRepository,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<NewsLetterSubscription> subscriptionRepository,
            IStoreContext storeContext,
            ILanguageService languageService,
            IStoreMappingService storeMappingService,
            IStaticCacheManager staticCacheManager)
        {
            _manufacturerRepository = manufacturerRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _subscriptionRepository = subscriptionRepository;
            _storeContext = storeContext;
            _languageService = languageService;
            _storeMappingService = storeMappingService;
            _cacheManager = staticCacheManager;
        }

        public Manufacturer GetManufacturerById(int manufacturerId)
        {
            if (manufacturerId == 0)
                return null;

            var manufacturer = _manufacturerRepository.Table.FirstOrDefault(m => m.Id == manufacturerId && !m.Deleted);

            return manufacturer;
        }

        public int GetManufacturersCount()
        {
            return _manufacturerRepository.Table.Count(manufacturer => !manufacturer.Deleted);
        }

        public IList<Manufacturer> GetManufacturers(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null, int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetManufacturersQuery(createdAtMin, createdAtMax, ids);

            if (sinceId > 0)
            {
                query = query.Where(order => order.Id > sinceId);
            }

            return new ApiList<Manufacturer>(query, page - 1, limit);
        }

        public IList<Manufacturer> Search(string queryParams = "", string order = Configurations.DefaultOrder, int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit)
        {
            var searchParams = queryParams.EnsureSearchQueryIsValid(parser => parser.ParseSearchQuery());

            if (searchParams != null)
            {
                var query = _manufacturerRepository.Table.Where(manufacturer => !manufacturer.Deleted);

                foreach (var searchParam in searchParams)
                {
                    // Skip non existing properties.
                    if (ReflectionHelper.HasProperty(searchParam.Key, typeof(Manufacturer)))
                    {
                        // @0 is a placeholder used by dynamic linq and it is used to prevent possible sql injections.
                        query = query.Where(string.Format("{0} = @0 || {0}.Contains(@0)", searchParam.Key), searchParam.Value);
                    }
                }

                return new ApiList<Manufacturer>(query, page - 1, limit);
            }

            return new List<Manufacturer>();
        }

        private IQueryable<Manufacturer> GetManufacturersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, IList<int> ids = null)
        {
            var query = _manufacturerRepository.Table;

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(manufacturer => ids.Contains(manufacturer.Id));
            }

            query = query.Where(manufacturer => !manufacturer.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(manufacturer => manufacturer.CreatedOnUtc > createdAtMin.Value.ToUniversalTime());
            }

            if (createdAtMax != null)
            {
                query = query.Where(manufacturer => manufacturer.CreatedOnUtc < createdAtMax.Value.ToUniversalTime());
            }

            query = query.OrderBy(order => order.Id);

            return query;
        }
    }
}
