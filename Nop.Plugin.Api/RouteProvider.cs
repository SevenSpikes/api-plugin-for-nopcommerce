using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

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
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
