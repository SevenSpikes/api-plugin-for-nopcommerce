// -----------------------------------------------------------------------
// <copyright from="2018" to="2018" file="ApiPlugin.cs" company="Lindell Technologies">
//    Copyright (c) Lindell Technologies All Rights Reserved.
//    Information Contained Herein is Proprietary and Confidential.
// </copyright>
// -----------------------------------------------------------------------

using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Api.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Api
{
    public class ApiPlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public ApiPlugin(ISettingService settingService, IWorkContext workContext,
            ILocalizationService localizationService, IWebHelper webHelper)
        {
            _settingService = settingService;
            _workContext = workContext;
            _localizationService = localizationService;
            _webHelper = webHelper;
        }

        public override void Install()
        {
            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients",
                "Manage Api Clients");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings",
                "General Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint",
                "By checking this settings you can Enable/Disable the Web Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger",
                "Allow Requests From Swagger");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint",
                "Swagger is the documentation generation tool used for the API (/Swagger). It has a client that enables it to make GET requests to the API endpoints. By enabling this option you will allow all requests from the swagger client. Do Not Enable on live site, it is only for demo sites or local testing!!!");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Title", "API");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title", "Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Clients.Title", "Clients");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Docs.Title", "Docs");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Settings.Title",
                "Api Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Title", "Api Clients");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Create.Title",
                "Add a new Api client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Edit.Title",
                "Edit Api client");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name", "Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name.Hint", "Name Hint");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId", "Client Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId.Hint",
                "The id of the client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret",
                "Client Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret.Hint",
                "The client secret is used during the authentication for obtaining the Access Token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl",
                "Callback Url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl.Hint",
                "The url where the Authorization code will be send");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive", "Is Active");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive.Hint",
                "You can use it to enable/disable the access to your store for the client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.AddNew", "Add New Client");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Edit", "Edit");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Created", "Created");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Deleted", "Deleted");
            _localizationService.AddOrUpdatePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name", "Name is required");
            _localizationService.AddOrUpdatePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId", "Client Id is required");
            _localizationService.AddOrUpdatePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret", "Client Secret is required");
            _localizationService.AddOrUpdatePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl", "Callback Url is required");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle",
                "General Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Edit", "Edit");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.BackToList", "Back To List");

            _localizationService.AddOrUpdatePluginLocaleResource("Api.Categories.Fields.Id.Invalid", "Id is invalid");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidPropertyType", "Invalid Property Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidType", "Invalid {0} type");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidRequest", "Invalid request");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidRootProperty", "Invalid root property");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.NoJsonProvided", "No Json provided");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidJsonFormat", "Json format is invalid");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageAttachmentFormat",
                "Invalid image attachment base64 format");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrc",
                "Invalid image source");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrcType",
                "You have provided an invalid image source/attachment ");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging", "Enable Logging");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging.Hint",
                "By enable logging you will see webhook messages in the Log. These messages are needed ONLY for diagnostic purposes. NOTE: A restart is required when changing this setting in order to take effect");

            var settings = new ApiSettings
            {
                EnableApi = true,
                AllowRequestsFromSwagger = false
            };

            _settingService.SaveSetting(settings);

            base.Install();
        }

        public override void Uninstall()
        {
            var persistedGrantMigrator =
                EngineContext.Current.Resolve<PersistedGrantDbContext>().GetService<IMigrator>();
            persistedGrantMigrator.Migrate("0");

            var configurationMigrator = EngineContext.Current.Resolve<ConfigurationDbContext>().GetService<IMigrator>();
            configurationMigrator.Migrate("0");

            // TODO: Delete all resources
            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Api");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients");

            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Clients.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Docs.Title");

            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Configure");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.GeneralSettings");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Name");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientId");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.IsActive");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Edit");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Created");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.Deleted");
            _localizationService.DeletePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name");
            _localizationService.DeletePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId");
            _localizationService.DeletePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret");
            _localizationService.DeletePluginLocaleResource(
                "Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Edit");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.Client.BackToList");

            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableLogging");
            _localizationService.DeletePluginLocaleResource("Plugins.Api.Admin.EnableLogging.Hint");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var pluginMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Title",
                _workContext.WorkingLanguage.Id, defaultValue: "API");

            var settingsMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Settings.Title",
                _workContext.WorkingLanguage.Id, defaultValue: "API");

            var manageClientsMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Clients.Title",
                _workContext.WorkingLanguage.Id, defaultValue: "API");

            const string adminUrlPart = "Admin/";

            var pluginMainMenu = new SiteMapNode
            {
                Title = pluginMenuName,
                Visible = true,
                SystemName = "Api-Main-Menu",
                IconClass = "fa-genderless"
            };

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = settingsMenuName,
                Url = _webHelper.GetStoreLocation() + adminUrlPart + "ApiAdmin/Settings",
                Visible = true,
                SystemName = "Api-Settings-Menu",
                IconClass = "fa-genderless"
            });

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = manageClientsMenuName,
                Url = _webHelper.GetStoreLocation() + adminUrlPart + "ManageClientsAdmin/List",
                Visible = true,
                SystemName = "Api-Clients-Menu",
                IconClass = "fa-genderless"
            });


            var pluginDocumentationUrl = "https://github.com/SevenSpikes/api-plugin-for-nopcommerce";

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = _localizationService.GetResource("Plugins.Api.Admin.Menu.Docs.Title"),
                Url = pluginDocumentationUrl,
                Visible = true,
                SystemName = "Api-Docs-Menu",
                IconClass = "fa-genderless"
            }); //TODO: target="_blank"


            rootNode.ChildNodes.Add(pluginMainMenu);
        }
    }
}