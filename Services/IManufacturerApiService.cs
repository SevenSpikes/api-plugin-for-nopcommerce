using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using static Nop.Plugin.Api.Infrastructure.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface IManufacturerApiService
    {
        Manufacturer GetManufacturerById(int manufacturerId);

        IList<Manufacturer> GetManufacturers(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId, 
            int? productId = null, bool? publishedStatus = null, int? languageId = null);

        int GetManufacturersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null);
    }
}