using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Infrastructure
{
    using System;
    using Nop.Core.Domain.Catalog;
    using Nop.Core.Domain.Common;
    using Nop.Core.Domain.Customers;
    using Nop.Core.Domain.Orders;
    using Nop.Plugin.Api.Controllers;
    using Nop.Plugin.Api.Converters;
    using Nop.Plugin.Api.Factories;
    using Nop.Plugin.Api.Helpers;
    using Nop.Plugin.Api.ModelBinders;
    using Nop.Plugin.Api.Serializers;
    using Nop.Plugin.Api.Validators;

    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            RegisterPluginServices(builder);

            RegisterControllers(builder);

            RegisterModelBinders(builder);
        }

        private void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterType<CustomersController>().InstancePerLifetimeScope();
            builder.RegisterType<CategoriesController>().InstancePerLifetimeScope();
            builder.RegisterType<ProductsController>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCategoryMappingsController>().InstancePerLifetimeScope();
            builder.RegisterType<OrdersController>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemsController>().InstancePerLifetimeScope();
            builder.RegisterType<OrderItemsController>().InstancePerLifetimeScope();
            builder.RegisterType<StoreController>().InstancePerLifetimeScope();
            builder.RegisterType<LanguagesController>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRolesController>().InstancePerLifetimeScope();
        }

        private void RegisterModelBinders(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ParametersModelBinder<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(JsonModelBinder<>)).InstancePerLifetimeScope();
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
            builder.RegisterType<ProductAttributeConverter>().As<IProductAttributeConverter>().InstancePerLifetimeScope();

            builder.RegisterType<MappingHelper>().As<IMappingHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JsonHelper>().As<IJsonHelper>().InstancePerLifetimeScope();
            builder.RegisterType<WebConfigMangerHelper>().As<IWebConfigMangerHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DTOHelper>().As<IDTOHelper>().InstancePerLifetimeScope();

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

        public virtual int Order
        {
            get { return Int16.MaxValue; }
        }
    }
}