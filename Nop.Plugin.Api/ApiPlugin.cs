namespace Nop.Plugin.Api
{
    using IdentityServer4.EntityFramework.DbContexts;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Nop.Core;
    using Nop.Core.Infrastructure;
    using Nop.Core.Plugins;
    using Nop.Plugin.Api.Data;
    using Nop.Plugin.Api.Domain;
    using Nop.Plugin.Api.Helpers;
    using Nop.Services.Configuration;
    using Nop.Services.Localization;
    using Nop.Web.Framework.Menu;

    public class ApiPlugin : BasePlugin, IAdminMenuPlugin
    {
        //private readonly IWebConfigMangerHelper _webConfigMangerHelper;
        private readonly ApiObjectContext _objectContext;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public ApiPlugin(ApiObjectContext objectContext,/*IWebConfigMangerHelper webConfigMangerHelper,*/ ISettingService settingService, IWorkContext workContext,
            ILocalizationService localizationService, IWebHelper webHelper
/*, IConfiguration configuration*/)
        {
            _objectContext = objectContext;
            //_webConfigMangerHelper = webConfigMangerHelper;
            _settingService = settingService;
            _workContext = workContext;
            _localizationService = localizationService;
            _webHelper = webHelper;
            //_configuration = configuration;
        }

        //private readonly IConfiguration _configuration;

        public override void Install()
        {
            var configManagerHelper = new NopConfigManagerHelper();

            // some of third party libaries that we use for WebHooks and Swagger use older versions
            // of certain assemblies so we need to redirect them to the those that nopCommerce uses
            configManagerHelper.AddBindingRedirects();

            // required by the WebHooks support
            configManagerHelper.AddConnectionString();

            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger", "Allow Requests From Swagger");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.AllowRequestsFromSwagger.Hint", "Swagger is the documentation generation tool used for the API (/Swagger). It has a client that enables it to make GET requests to the API endpoints. By enabling this option you will allow all requests from the swagger client. Do Not Enable on live site, it is only for demo sites or local testing!!!");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Title","API");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title","Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Clients.Title", "Clients");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Docs.Title", "Docs");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Settings.Title", "Api Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Title", "Api Clients");
            
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Create.Title", "Add a new Api client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Clients.Edit.Title", "Edit Api client");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.Name.Hint", "Name Hint");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId", "Client Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientId.Hint", "The id of the client");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret", "Client Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.ClientSecret.Hint", "The client secret is used during the authentication for obtaining the Access Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl", "Callback Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.CallbackUrl.Hint", "The url where the Authorization code will be send");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive", "Is Active");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Client.IsActive.Hint", "You can use it to enable/disable the access to your store for the client");
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

            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotRegisterWebhook", "Could not register WebHook due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook", "Could not register WebHook because a webhook with the same URI and Filters is already registered.");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotUpdateWebhook", "Could not update WebHook due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhook", "Could not delete WebHook due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhooks", "Could not delete WebHooks due to error: {0}");
            this.AddOrUpdatePluginLocaleResource("Api.WebHooks.InvalidFilters", "The following filters are not valid: '{0}'. A list of valid filters can be obtained from the path '{1}'.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging", "Enable Logging");
            this.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableLogging.Hint", "By enable logging you will see webhook messages in the Log. These messages are needed ONLY for diagnostic purposes. NOTE: A restart is required when changing this setting in order to take effect");

            ApiSettings settings = new ApiSettings
            {
                EnableApi = true,
                AllowRequestsFromSwagger = false
            };

            _settingService.SaveSetting(settings);           

            base.Install();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the Install function, but just to be safe we will made web.config changes after the plugin was installed.
            //_webConfigMangerHelper.AddConfiguration();
        }

        public override void Uninstall()
        {
            _objectContext.Uninstall();

            var persistedGrantMigrator = EngineContext.Current.Resolve<PersistedGrantDbContext>().GetService<IMigrator>();
            persistedGrantMigrator.Migrate("0");
            
            var configurationMigrator = EngineContext.Current.Resolve<ConfigurationDbContext>().GetService<IMigrator>();
            configurationMigrator.Migrate("0");

            // TODO: Delete all resources
            //locales
            this.DeletePluginLocaleResource("Plugins.Api");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients");

            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Title");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Clients.Title");
            this.DeletePluginLocaleResource("Plugins.Api.Admin.Menu.Docs.Title");

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
            
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotRegisterWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotUpdateWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhook");
            this.DeletePluginLocaleResource("Api.WebHooks.CouldNotDeleteWebhooks");
            this.DeletePluginLocaleResource("Api.WebHooks.InvalidFilters");

            base.Uninstall();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the uninstall function, but just to be safe we will made web.config changes after the plugin was uninstalled.
            //_webConfigMangerHelper.RemoveConfiguration();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            string pluginMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Title",languageId: _workContext.WorkingLanguage.Id, defaultValue: "API");

            string settingsMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Settings.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "API");

            string manageClientsMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Clients.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "API");

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
            

            string pluginDocumentationUrl = "https://github.com/SevenSpikes/api-plugin-for-nopcommerce";
            
            pluginMainMenu.ChildNodes.Add(new SiteMapNode
                {
                    Title = _localizationService.GetResource("Plugins.Api.Admin.Menu.Docs.Title"),
                    Url = pluginDocumentationUrl,
                    Visible = true,
                    SystemName = "Api-Docs-Menu",
                    IconClass = "fa-genderless"
                });//TODO: target="_blank"
            

            rootNode.ChildNodes.Add(pluginMainMenu);
        }
    }
}
