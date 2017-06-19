using AutoMapper;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.DTOs.Stores;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class StoreDtoMappings
    {
        public static StoreDto ToDto(this Store store)
        {
            return Mapper.DynamicMap<Store, StoreDto>(store);
        }
    }
}
