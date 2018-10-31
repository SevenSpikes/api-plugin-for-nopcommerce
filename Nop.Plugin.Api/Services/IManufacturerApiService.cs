using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface IManufacturerApiService
    {
        int GetManufacturersCount();

        Manufacturer GetManufacturerById(int manufacturerId);

        IList<Manufacturer> GetManufacturers(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                               int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
                               int sinceId = Configurations.DefaultSinceId);

        IList<Manufacturer> Search(string queryParams = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit);
    }
}
