using AutoMapper;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.Customers;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class CustomerDtoMappings
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return Mapper.Map<Customer, CustomerDto>(customer);
        }

        public static OrderCustomerDto ToOrderCustomerDto(this Customer customer)
        {
            return Mapper.Map<Customer, OrderCustomerDto>(customer);
        }

        public static CustomerForShoppingCartItemDto ToCustomerForShoppingCartItemDto(this Customer customer)
        {
            return Mapper.DynamicMap<Customer, CustomerForShoppingCartItemDto>(customer);
        }
    }
}