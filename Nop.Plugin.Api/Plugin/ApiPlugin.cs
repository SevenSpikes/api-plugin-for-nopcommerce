using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Api.Data;
using Nop.Plugin.Api.Helpers;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Plugin
{
    public class ApiPlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
    {
        private const string ControllersNamespace = "Nop.Plugin.Api.Controllers";

        private readonly ApiObjectContext _objectContext;
        private readonly IWebConfigMangerHelper _webConfigMangerHelper;

        public ApiPlugin(ApiObjectContext objectContext, IWebConfigMangerHelper webConfigMangerHelper)
        {
            _objectContext = objectContext;
            _webConfigMangerHelper = webConfigMangerHelper;
        }

        public override void Install()
        {
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger", "Allow Requests From Swagger");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint", "Swagger is the documentation generation tool used for the API. It has a client that enables it to make requests to the api endpoints, so the users can try our certain point on place. By enabling this option you will allow all requests from the swagger client. Do Not Enable on live site, it is only for demo sites or local testing!!!");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId", "Client Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret", "Client Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl", "Callback Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive", "Is Active");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.AddNew", "Add New Client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Created", "Created");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Deleted", "Deleted");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name", "Name is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId", "Client Id is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret", "Client Secret is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl", "Callback Url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.BackToList", "Back To List");

            this.AddOrUpdatePluginLocaleResource("Api.Categories.Fields.Id.Invalid", "Id is invalid");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidPropertyType", "Invalid Property Type");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidType", "Invalid {0} type");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidRequest", "Invalid request");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidRootProperty", "Invalid root property");
            this.AddOrUpdatePluginLocaleResource("Api.NoJsonProvided", "No Json provided");
            this.AddOrUpdatePluginLocaleResource("Api.InvalidJsonFormat", "Json format is invalid");
            this.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageAttachmentFormat", "Invalid image attachment base64 format");
            this.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrc", "Invalid image source");
            this.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrcType", "You have provided an invalid image source/attachment ");

            base.Install();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the Install function, but just to be safe we will made web.config changes after the plugin was installed.
            _webConfigMangerHelper.AddConfiguration();
        }

        public override void Uninstall()
        {
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Api");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Configure");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.GeneralSettings");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint");

            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Name");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientId");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.IsActive");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.AddNew");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Edit");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Created");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Deleted");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Edit");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Client.BackToList");

            base.Uninstall();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the uninstall function, but just to be safe we will made web.config changes after the plugin was uninstalled.
            _webConfigMangerHelper.RemoveConfiguration();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
           
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ApiAdmin";
            routeValues = new RouteValueDictionary { { "Namespaces", ControllersNamespace }, { "area", null } };
        }
    }
}
