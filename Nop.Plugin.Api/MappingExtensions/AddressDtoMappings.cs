using AutoMapper;
using Nop.Core.Domain.Common;
using Nop.Plugin.Api.DTOs;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class AddressDtoMappings
    {
        public static AddressDto ToDto(this Address address)
        {
            return Mapper.DynamicMap<Address, AddressDto>(address);
        }

        public static Address ToEntity(this AddressDto addressDto)
        {
            return Mapper.DynamicMap<AddressDto, Address>(addressDto);
        }
    }
}