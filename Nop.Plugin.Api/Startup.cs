using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Routing;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Owin.Middleware;
using Nop.Plugin.Api.Owin.OAuth.Providers;
using Nop.Plugin.Api.Swagger;
using Owin;
using Swashbuckle.Application;

namespace Nop.Plugin.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureOAuth(app);

            app.UseStageMarker(PipelineStage.PostAuthenticate);

            ConfigureWebApi(app);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            // The token endpoint path activates the ValidateClientAuthentication method from the AuthorisationServerProvider.
            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AuthorizeEndpointPath = new PathString("/OAuth/Authorize"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Configurations.AccessTokenExpirationMinutes),
                Provider = new AuthorisationServerProvider(),
                AuthorizationCodeProvider = new AuthenticationTokenProvider(),
                RefreshTokenProvider = new RefreshTokenProvider(),
                ApplicationCanDisplayErrors = true
            };
            app.UseOAuthAuthorizationServer(oAuthServerOptions);


            // Our own middleware that resets the current user set by the Forms authentication in case we have a Bearer token request
            app.Use(typeof(BearerTokenMiddleware));

            // This middleware should be called after the BearerTokenMiddleware
            app.Use(typeof(OAuthBearerAuthenticationMiddleware), app, new OAuthBearerAuthenticationOptions());
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Filters.Add(new ServerErrorHandlerAttribute());

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            config.Routes.MapHttpRoute(
                name: "authorizeApi",
                routeTemplate: "OAuth/Authorize",
                defaults: new { controller = "OAuth", action = "Authorize" });

            config.Routes.MapHttpRoute(
              name: "customers",
              routeTemplate: "api/customers",
              defaults: new { controller = "Customers", action = "GetCustomers" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "customersCount",
                routeTemplate: "api/customers/count",
                defaults: new { controller = "Customers", action = "GetCustomersCount" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "customerSearch",
                routeTemplate: "api/customers/search",
                defaults: new { controller = "Customers", action = "Search" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "customerById",
                routeTemplate: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "GetCustomerById" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
              name: "createCustomer",
              routeTemplate: "api/customers",
              defaults: new { controller = "Customers", action = "CreateCustomer" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
                name: "updateCustomer",
                routeTemplate: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "UpdateCustomer" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute(
                name: "deleteCustomer",
                routeTemplate: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "DeleteCustomer" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute(
               name: "categories",
               routeTemplate: "api/categories",
               defaults: new { controller = "Categories", action = "GetCategories" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
               name: "deleteCategory",
               routeTemplate: "api/categories/{id}",
               defaults: new { controller = "Categories", action = "DeleteCategory" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute(
               name: "createCategory",
               routeTemplate: "api/categories",
               defaults: new { controller = "Categories", action = "CreateCategory" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
               name: "updateCategory",
               routeTemplate: "api/categories/{id}",
               defaults: new { controller = "Categories", action = "UpdateCategory" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute(
                name: "categoriesCount",
                routeTemplate: "api/categories/count",
                defaults: new { controller = "Categories", action = "GetCategoriesCount" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "categoryById",
                routeTemplate: "api/categories/{id}",
                defaults: new { controller = "Categories", action = "GetCategoryById" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "products",
                routeTemplate: "api/products",
                defaults: new { controller = "Products", action = "GetProducts" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
              name: "createProduct",
              routeTemplate: "api/products",
              defaults: new { controller = "Products", action = "CreateProduct" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
              name: "updateProduct",
              routeTemplate: "api/products/{id}",
              defaults: new { controller = "Products", action = "UpdateProduct" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute(
              name: "deleteProduct",
              routeTemplate: "api/products/{id}",
              defaults: new { controller = "Products", action = "DeleteProduct" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute(
                name: "productsCount",
                routeTemplate: "api/products/count",
                defaults: new { controller = "Products", action = "GetProductsCount" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "productById",
                routeTemplate: "api/products/{id}",
                defaults: new { controller = "Products", action = "GetProductById" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
             name: "orders",
             routeTemplate: "api/orders",
             defaults: new { controller = "Orders", action = "GetOrders" },
             constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "ordersCount",
                routeTemplate: "api/orders/count",
                defaults: new { controller = "Orders", action = "GetOrdersCount" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "ordersByCustomerId",
                routeTemplate: "api/orders/customer/{customer_id}",
                defaults: new { controller = "Orders", action = "GetOrdersByCustomerId" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "orderById",
                routeTemplate: "api/orders/{id}",
                defaults: new { controller = "Orders", action = "GetOrderById" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "createOrder",
                routeTemplate: "api/orders",
                defaults: new { controller = "Orders", action = "CreateOrder" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
                name: "updateOrder",
                routeTemplate: "api/orders/{id}",
                defaults: new { controller = "Orders", action = "UpdateOrder" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute(
               name: "deleteOrder",
               routeTemplate: "api/orders/{id}",
               defaults: new { controller = "Orders", action = "DeleteOrder" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute(
              name: "productCategoryMappings",
              routeTemplate: "api/product_category_mappings",
              defaults: new { controller = "ProductCategoryMappings", action = "GetMappings" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
              name: "createProductCategoryMappings",
              routeTemplate: "api/product_category_mappings",
              defaults: new { controller = "ProductCategoryMappings", action = "CreateProductCategoryMapping" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
              name: "updateProductCategoryMapping",
              routeTemplate: "api/product_category_mappings/{id}",
              defaults: new { controller = "ProductCategoryMappings", action = "UpdateProductCategoryMapping" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute(
               name: "deleteProductCategoryMapping",
               routeTemplate: "api/product_category_mappings/{id}",
               defaults: new { controller = "ProductCategoryMappings", action = "DeleteProductCategoryMapping" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute(
                name: "productCategoryMappingsCount",
                routeTemplate: "api/product_category_mappings/count",
                defaults: new { controller = "ProductCategoryMappings", action = "GetMappingsCount" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "productCategoryMappingById",
                routeTemplate: "api/product_category_mappings/{id}",
                defaults: new { controller = "ProductCategoryMappings", action = "GetMappingById" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
               name: "createShoppingCartItem",
               routeTemplate: "api/shopping_cart_items",
               defaults: new { controller = "ShoppingCartItems", action = "CreateShoppingCartItem" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
              name: "updateShoppingCartItem",
              routeTemplate: "api/shopping_cart_items/{id}",
              defaults: new { controller = "ShoppingCartItems", action = "UpdateShoppingCartItem" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute(
              name: "deleteShoppingCartItem",
              routeTemplate: "api/shopping_cart_items/{id}",
              defaults: new { controller = "ShoppingCartItems", action = "DeleteShoppingCartItem" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute(
                name: "shoppingCartItems",
                routeTemplate: "api/shopping_cart_items",
                defaults: new { controller = "ShoppingCartItems", action = "GetShoppingCartItems" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "shoppingCartItemsByCustomerId",
                routeTemplate: "api/shopping_cart_items/{customerId}",
                defaults: new { controller = "ShoppingCartItems", action = "GetShoppingCartItemsByCustomerId" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
               name: "orderItemsByOrderId",
               routeTemplate: "api/orders/{orderId}/items",
               defaults: new { controller = "OrderItems", action = "GetOrderItems" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
              name: "orderItemByIdForOrder",
              routeTemplate: "api/orders/{orderId}/items/{orderItemId}",
              defaults: new { controller = "OrderItems", action = "GetOrderItemByIdForOrder" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
               name: "orderItemsCountByOrderId",
               routeTemplate: "api/orders/{orderId}/items/count",
               defaults: new { controller = "OrderItems", action = "GetOrderItemsCount" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                name: "CreateOrderItem",
                routeTemplate: "api/orders/{orderId}/items",
                defaults: new { controller = "OrderItems", action = "CreateOrderItem" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
              name: "UpdateOrderItem",
              routeTemplate: "api/orders/{orderId}/items/{orderItemId}",
              defaults: new { controller = "OrderItems", action = "UpdateOrderItem" },
              constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute(
               name: "deleteOrderItemForOrderById",
               routeTemplate: "api/orders/{orderId}/items/{orderItemId}",
               defaults: new { controller = "OrderItems", action = "DeleteOrderItemById" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute(
               name: "deleteOrderItemsForOrder",
               routeTemplate: "api/orders/{orderId}/items",
               defaults: new { controller = "OrderItems", action = "DeleteAllOrderItemsForOrder" },
               constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            // The default route templates for the Swagger docs and swagger-ui are "swagger/docs/{apiVersion}" and "swagger/ui/index#/{assetPath}" respectively.
            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "RESTful API documentation");
                    c.IncludeXmlComments(string.Format(@"{0}\Plugins\Nop.Plugin.Api\Nop.Plugin.Api.XML", AppDomain.CurrentDomain.BaseDirectory));
                    // We need this filter to exclude some of the API endpoints from the documentation i.e /OAuth/Authorize endpoint
                    c.DocumentFilter<ExcludeEnpointsDocumentFilter>();
                    c.OperationFilter<RemovePrefixesOperationFilter>();
                    c.OperationFilter<ChangeParameterTypeOperationFilter>();
                })
                .EnableSwaggerUi(c =>
                {
                    var currentAssembly = Assembly.GetAssembly(this.GetType());
                    var currentAssemblyName = currentAssembly.GetName().Name;

                    // Needeed for removing the "Try It Out" button from the post and put methods.
                    // http://stackoverflow.com/questions/36772032/swagger-5-2-3-supportedsubmitmethods-removed/36780806#36780806
                    c.InjectJavaScript(currentAssembly, string.Format("{0}.Scripts.swaggerPostPutTryItOutButtonsRemoval.js", currentAssemblyName));
                });

            app.UseWebApi(config);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(EngineContext.Current.ContainerManager.Container);
        }
    }
}

