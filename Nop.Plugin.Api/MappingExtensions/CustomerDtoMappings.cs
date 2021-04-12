using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.Customers;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class CustomerDtoMappings
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return customer.MapTo<Customer, CustomerDto>();
        }

        public static OrderCustomerDto ToOrderCustomerDto(this Customer customer)
        {
            return customer.MapTo<Customer, OrderCustomerDto>();
        }

        public static OrderCustomerDto ToOrderCustomerDto(this CustomerDto customerDto)
        {
            return customerDto.MapTo<CustomerDto, OrderCustomerDto>();
        }

        public static CustomerForShoppingCartItemDto ToCustomerForShoppingCartItemDto(this Customer customer)
        {
            return customer.MapTo<Customer, CustomerForShoppingCartItemDto>();
        }
    }
}
