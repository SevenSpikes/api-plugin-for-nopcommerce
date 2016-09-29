using System;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.Converters;
using Nop.Plugin.Api.Data;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Api.Infrastructure
{
    public class DependencyRegister : IDependencyRegistrar
    {
		private const string ObjectContextName = "nop_object_context_web_api";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            this.RegisterPluginDataContext<ApiObjectContext>(builder, ObjectContextName);

            builder.RegisterType<EfRepository<Client>>()
               .As<IRepository<Client>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ObjectContextName))
               .InstancePerLifetimeScope();

            CreateModelMappings();

            RegisterPluginServices(builder);

            RegisterControllers(builder);

            RegisterModelBinders(builder);
        }

        private void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterType<CustomersController>().InstancePerLifetimeScope();
            builder.RegisterType<CategoriesApiController>().InstancePerLifetimeScope();
            builder.RegisterType<ProductsController>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCategoryMappingsController>().InstancePerLifetimeScope();
            builder.RegisterType<OrdersController>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemsController>().InstancePerLifetimeScope();
            builder.RegisterType<OrderItemsController>().InstancePerLifetimeScope();
        }

        private void RegisterModelBinders(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ParametersModelBinder<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(JsonModelBinder<>)).InstancePerLifetimeScope();
        }

        private void CreateModelMappings()
        {
            MappingExtensions.Maps.CreateMap<ApiSettings, ConfigurationModel>();
            MappingExtensions.Maps.CreateMap<ConfigurationModel, ApiSettings>();
            
            MappingExtensions.Maps.CreateMap<Client, ClientModel>();
            MappingExtensions.Maps.CreateMap<ClientModel, Client>();
            
            MappingExtensions.Maps.CreateMap<Category, CategoryDto>();
            MappingExtensions.Maps.CreateMap<CategoryDto, Category>();
            
            MappingExtensions.Maps.CreateMap<ProductCategory, ProductCategoryMappingDto>();
            
            MappingExtensions.Maps.CreateAddressMap();
            MappingExtensions.Maps.CreateAddressDtoToEntityMap();
            MappingExtensions.Maps.CreateShoppingCartItemMap();
            
            MappingExtensions.Maps.CreateCustomerToDTOMap();
            MappingExtensions.Maps.CreateCustomerToOrderCustomerDTOMap();
            MappingExtensions.Maps.CreateCustomerForShoppingCartItemMapFromCustomer();

            MappingExtensions.Maps.CreateMap<OrderItem, OrderItemDto>();
            MappingExtensions.Maps.CreateOrderEntityToOrderDtoMap();

            MappingExtensions.Maps.CreateProductMap();
        }

        private void RegisterPluginServices(ContainerBuilder builder)
        {
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerApiService>().As<ICustomerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryApiService>().As<ICategoryApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductApiService>().As<IProductApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCategoryMappingsApiService>().As<IProductCategoryMappingsApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderApiService>().As<IOrderApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemApiService>().As<IShoppingCartItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderItemApiService>().As<IOrderItemApiService>().InstancePerLifetimeScope();

            builder.RegisterType<MappingHelper>().As<IMappingHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationHelper>().As<IAuthorizationHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JsonHelper>().As<IJsonHelper>().InstancePerLifetimeScope();
            builder.RegisterType<WebConfigMangerHelper>().As<IWebConfigMangerHelper>().InstancePerLifetimeScope();

            builder.RegisterType<JsonFieldsSerializer>().As<IJsonFieldsSerializer>().InstancePerLifetimeScope();

            builder.RegisterType<FieldsValidator>().As<IFieldsValidator>().InstancePerLifetimeScope();

            builder.RegisterType<ObjectConverter>().As<IObjectConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ApiTypeConverter>().As<IApiTypeConverter>().InstancePerLifetimeScope();

            builder.RegisterType<CategoryFactory>().As<IFactory<Category>>().InstancePerLifetimeScope();
            builder.RegisterType<ProductFactory>().As<IFactory<Product>>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerFactory>().As<IFactory<Customer>>().InstancePerLifetimeScope();
            builder.RegisterType<AddressFactory>().As<IFactory<Address>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderFactory>().As<IFactory<Order>>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemFactory>().As<IFactory<ShoppingCartItem>>().InstancePerLifetimeScope();

            builder.RegisterType<Maps.JsonPropertyMapper>().As<Maps.IJsonPropertyMapper>().InstancePerLifetimeScope();
        }

        public int Order { get; }
    }
}