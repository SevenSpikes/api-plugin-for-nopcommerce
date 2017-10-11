using System.Collections.Generic;
using AutoMapper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Customers;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.CustomerRoles;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.Models;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class Maps
    {
        public static IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            return AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>().IgnoreAllNonExisting();
        }

        public static void CreateOrderEntityToOrderDtoMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Order, OrderDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.OrderItemDtos, y => y.MapFrom(src => src.OrderItems.Select(x => x.ToDto())));
        }
        
        public static void CreateAddressMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Address, AddressDto>()
               .IgnoreAllNonExisting()
               .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
               .ForMember(x => x.CountryName, y => y.MapFrom(src => src.Country.GetWithDefault(x => x, new Country()).Name))
               .ForMember(x => x.StateProvinceName, y => y.MapFrom(src => src.StateProvince.GetWithDefault(x => x, new StateProvince()).Name));
        }

        public static void CreateAddressDtoToEntityMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<AddressDto, Address>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id));
        }

        public static void CreateCustomerForShoppingCartItemMapFromCustomer()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Customer, CustomerForShoppingCartItemDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.BillingAddress, y => y.MapFrom(src => src.BillingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.ShippingAddress, y => y.MapFrom(src => src.ShippingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.Addresses, y => y.MapFrom(src => src.Addresses.GetWithDefault(x => x, new List<Address>()).Select(address => address.ToDto())));
        }
        
        public static void CreateCustomerToDTOMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Customer, CustomerDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.BillingAddress,
                    y => y.MapFrom(src => src.BillingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.ShippingAddress,
                    y => y.MapFrom(src => src.ShippingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.CustomerAddresses,
                    y =>
                        y.MapFrom(
                            src =>
                                src.Addresses.GetWithDefault(x => x, new List<Address>())
                                    .Select(address => address.ToDto())))
                .ForMember(x => x.ShoppingCartItems,
                    y =>
                        y.MapFrom(
                            src =>
                                src.ShoppingCartItems.GetWithDefault(x => x, new List<ShoppingCartItem>())
                                    .Select(item => item.ToDto())))
                 .ForMember(x => x.RoleIds, y => y.MapFrom(src => src.CustomerRoles.Select(z => z.Id)));
        }

        public static void CreateCustomerToOrderCustomerDTOMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Customer, OrderCustomerDto>()
                .IgnoreAllNonExisting();
        }

        public static void CreateCustomerDTOToOrderCustomerDTOMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<CustomerDto, OrderCustomerDto>()
                .IgnoreAllNonExisting();
        }

        public static void CreateShoppingCartItemMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.CustomerDto, y => y.MapFrom(src => src.Customer.GetWithDefault(x => x, new Customer()).ToCustomerForShoppingCartItemDto()))
                .ForMember(x => x.ProductDto, y => y.MapFrom(src => src.Product.GetWithDefault(x => x, new Product()).ToDto()));
        }

        public static void CreateProductMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Product, ProductDto>()
               .IgnoreAllNonExisting()
               .ForMember(x => x.ProductAttributeMappings, y => y.Ignore())
               .ForMember(x => x.FullDescription, y => y.MapFrom(src => HttpUtility.HtmlEncode(src.FullDescription)))
               .ForMember(x => x.Tags, y => y.MapFrom(src => src.ProductTags.Select(x => x.Name)));
        }

        public static void CreateAllMappings()
        {
            CreateMap<ApiSettings, ConfigurationModel>();
            CreateMap<ConfigurationModel, ApiSettings>();

            CreateMap<Client, ClientModel>();
            CreateMap<ClientModel, Client>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<Store, StoreDto>();

            CreateMap<ProductCategory, ProductCategoryMappingDto>();

            CreateMap<Language, LanguageDto>();

            CreateMap<CustomerRole, CustomerRoleDto>();

            CreateAddressMap();
            CreateAddressDtoToEntityMap();
            CreateShoppingCartItemMap();

            CreateCustomerToDTOMap();
            CreateCustomerToOrderCustomerDTOMap();
            CreateCustomerDTOToOrderCustomerDTOMap();
            CreateCustomerForShoppingCartItemMapFromCustomer();

            CreateMap<OrderItem, OrderItemDto>();
            CreateOrderEntityToOrderDtoMap();

            CreateProductMap();

            CreateMap<ProductAttributeValue, ProductAttributeValueDto>();
        }
    }
}