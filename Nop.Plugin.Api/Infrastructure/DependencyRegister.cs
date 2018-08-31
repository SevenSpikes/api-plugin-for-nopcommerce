using System;
using Autofac;
using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Api.Converters;
using Nop.Plugin.Api.Data;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Maps;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using Nop.Web.Framework.Infrastructure.Extensions;


namespace Nop.Plugin.Api.Infrastructure
{
    public class DependencyRegister : IDependencyRegistrar
    {
        private const string ObjectContextName = "nop_object_context_web_api";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            RegisterPluginServices(builder);

            RegisterModelBinders(builder);
        }

        public virtual int Order => 1; 

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
            builder.RegisterType<ProductCategoryMappingsApiService>().As<IProductCategoryMappingsApiService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OrderApiService>().As<IOrderApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemApiService>().As<IShoppingCartItemApiService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OrderItemApiService>().As<IOrderItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributesApiService>().As<IProductAttributesApiService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ProductPictureService>().As<IProductPictureService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeConverter>().As<IProductAttributeConverter>()
                .InstancePerLifetimeScope();
            builder.RegisterType<NewsLetterSubscriptionApiService>().As<INewsLetterSubscriptionApiService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MappingHelper>().As<IMappingHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JsonHelper>().As<IJsonHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DTOHelper>().As<IDTOHelper>().InstancePerLifetimeScope();
            builder.RegisterType<NopConfigManagerHelper>().As<IConfigManagerHelper>().InstancePerLifetimeScope();

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

            builder.RegisterType<JsonPropertyMapper>().As<IJsonPropertyMapper>().InstancePerLifetimeScope();

            // data access
            
            builder.RegisterPluginDataContext<ApiObjectContext>(ObjectContextName);
            

            // Repositories

            builder.RegisterType<EfRepository<Order>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ObjectContextName))
                .InstancePerLifetimeScope()
                .Named<IRepository<Order>>(ObjectContextName);

            builder.RegisterType<EfRepository<Customer>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ObjectContextName))
                .InstancePerLifetimeScope()
                .Named<IRepository<Customer>>(ObjectContextName);

            builder.RegisterType<EfRepository<GenericAttribute>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ObjectContextName))
                .InstancePerLifetimeScope()
                .Named<IRepository<GenericAttribute>>(ObjectContextName);

            builder.RegisterType<EfRepository<NewsLetterSubscription>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ObjectContextName))
                .InstancePerLifetimeScope()
                .Named<IRepository<NewsLetterSubscription>>(ObjectContextName);

            //TODO: Add the others


            // Services
            builder.RegisterType<OrderApiService>()
                    .As<IOrderApiService>()
                    .WithParameter(ResolvedParameter.ForNamed<IRepository<Order>>(ObjectContextName))
                    .InstancePerLifetimeScope();

            builder.RegisterType<CustomerApiService>()
                .As<ICustomerApiService>()
                .WithParameter(ResolvedParameter.ForNamed<IRepository<GenericAttribute>>(ObjectContextName))
                .WithParameter(ResolvedParameter.ForNamed<IRepository<Customer>>(ObjectContextName))
                .WithParameter(ResolvedParameter.ForNamed<IRepository<NewsLetterSubscription>>(ObjectContextName))
                .InstancePerLifetimeScope();


            //TODO: Add the others



        }

    }
}