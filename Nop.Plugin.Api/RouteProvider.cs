using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Plugin.Api.Helpers;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Api
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Api.Settings",
                 "Plugins/ApiAdmin/Settings",
                 new { controller = "ApiAdmin", action = "Settings", },
                 new[] { "Nop.Plugin.Api.Controllers.Admin" }
            );

            routes.MapRoute("Plugin.Api.ManageClients.List",
                 "Plugins/ManageClientsAdmin/List",
                 new { controller = "ManageClientsAdmin", action = "List" },
                 new[] { "Nop.Plugin.Api.Controllers.Admin" }
            );

            routes.MapRoute("Plugin.Api.ManageClients.Create",
                 "Plugins/ManageClientsAdmin/Create",
                 new { controller = "ManageClientsAdmin", action = "Create" },
                 new[] { "Nop.Plugin.Api.Controllers.Admin" }
            );

            routes.MapRoute("Plugin.Api.ManageClients.Edit",
                 "Plugins/ManageClientsAdmin/Edit/{id}",
                 new { controller = "ManageClientsAdmin", action = "Edit", id = @"\d+" },
                 new[] { "Nop.Plugin.Api.Controllers.Admin" }
            );

            routes.MapRoute("Plugin.Api.ManageClients.Delete",
                 "Plugins/ManageClientsAdmin/Delete/{id}",
                 new { controller = "ManageClientsAdmin", action = "Delete" , id = @"\d+" },
                 new[] { "Nop.Plugin.Api.Controllers.Admin" }
            );


            IWebConfigMangerHelper webConfigManagerHelper = EngineContext.Current.ContainerManager.Resolve<IWebConfigMangerHelper>();

            // make sure the connection string is added in the Web.config
            webConfigManagerHelper.AddConnectionString();

            // make sure the OwinAutomaticAppStartup is enabled in the Web.config
            webConfigManagerHelper.AddConfiguration();
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
