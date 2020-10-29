using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Domain;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Api.Infrastructure
{
    public class ApiPlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public ApiPlugin(
            ISettingService settingService,
            IWorkContext workContext,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IWebHelper webHelper)
        {
            _settingService = settingService;
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _webHelper = webHelper;
        }

        public override void Install()
        {
            //locales

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api", "Api plugin");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.ManageClients", "Manage Api Clients");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Configure", "Configure Web Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.GeneralSettings", "General Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi", "Enable Api");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.EnableApi.Hint", "By checking this settings you can Enable/Disable the Web Api");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Title", "API");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Menu.Settings.Title", "Settings");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Page.Settings.Title", "Api Settings");


            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Settings.GeneralSettingsTitle", "General Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Api.Admin.Edit", "Edit");

            _localizationService.AddOrUpdatePluginLocaleResource("Api.Categories.Fields.Id.Invalid", "Id is invalid");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidPropertyType", "Invalid Property Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidType", "Invalid {0} type");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidRequest", "Invalid request");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidRootProperty", "Invalid root property");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.NoJsonProvided", "No Json provided");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.InvalidJsonFormat", "Json format is invalid");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageAttachmentFormat", "Invalid image attachment base64 format");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrc", "Invalid image source");
            _localizationService.AddOrUpdatePluginLocaleResource("Api.Category.InvalidImageSrcType", "You have provided an invalid image source/attachment ");

            _settingService.SaveSetting(new ApiSettings());

            var apiRole = _customerService.GetCustomerRoleBySystemName(Constants.Roles.ApiRoleSystemName);

            if (apiRole == null)
            {
                apiRole = new CustomerRole
                {
                    Name = Constants.Roles.ApiRoleName,
                    Active = true,
                    SystemName = Constants.Roles.ApiRoleSystemName
                };

                _customerService.InsertCustomerRole(apiRole);
            }
            else if (apiRole.Active == false)
            {
                apiRole.Active = true;
                _customerService.UpdateCustomerRole(apiRole);
            }


            base.Install();

            // Changes to Web.Config trigger application restart.
            // This doesn't appear to affect the Install function, but just to be safe we will made web.config changes after the plugin was installed.
            //_webConfigMangerHelper.AddConfiguration();
        }

        public override void Uninstall()
        {
            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Api");

            var apiRole = _customerService.GetCustomerRoleBySystemName(Constants.Roles.ApiRoleSystemName);
            if (apiRole != null)
            {
                apiRole.Active = false;
                _customerService.UpdateCustomerRole(apiRole);
            }


            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var pluginMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Title", _workContext.WorkingLanguage.Id, defaultValue: "API");

            var settingsMenuName = _localizationService.GetResource("Plugins.Api.Admin.Menu.Settings.Title", _workContext.WorkingLanguage.Id, defaultValue: "API");

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


            rootNode.ChildNodes.Add(pluginMainMenu);
        }
    }
}
