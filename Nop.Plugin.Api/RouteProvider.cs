using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Api
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //routeBuilder.MapRoute("Plugin.Api.Settings",
            //    "Admin/ApiAdmin/Settings",
            //    new { controller = "ApiAdmin", action = "Settings" }
            //);

            //routeBuilder.MapRoute("Plugin.Api.ManageClients.List",
            //    "Admin/ManageClientsAdmin/List",
            //    new { controller = "ManageClientsAdmin", action = "List" }
            //);

            //routeBuilder.MapRoute("Plugin.Api.ManageClients.Create",
            //    "Admin/ManageClientsAdmin/Create",
            //    new { controller = "ManageClientsAdmin", action = "Create" }
            //);

            //routeBuilder.MapRoute("Plugin.Api.ManageClients.Edit",
            //    "Admin/ManageClientsAdmin/Edit/{clientId}",
            //    new { controller = "ManageClientsAdmin", action = "Edit", clientId = "" }
            //);

            //routeBuilder.MapRoute("Plugin.Api.ManageClients.Delete",
            //    "Admin/ManageClientsAdmin/Delete/{clientId}",
            //    new { controller = "ManageClientsAdmin", action = "Delete", clientId = "" }
            //);


            //IWebConfigMangerHelper webConfigManagerHelper = EngineContext.Current.Resolve<IWebConfigMangerHelper>();

            // make sure the connection string is added in the Web.config
            //webConfigManagerHelper.AddConnectionString();

            // make sure the OwinAutomaticAppStartup is enabled in the Web.config
            //webConfigManagerHelper.AddConfiguration();
        }

        public int Priority
        {
            get { return -1; }
        }
    }
}
