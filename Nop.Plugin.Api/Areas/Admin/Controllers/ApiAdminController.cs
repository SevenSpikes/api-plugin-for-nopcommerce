using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Api.Areas.Admin.Models;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Api.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class ApiAdminController : BasePluginController
    {
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public ApiAdminController(
            IStoreContext storeContext,
            ISettingService settingService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Settings()
        {
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var apiSettings = _settingService.LoadSetting<ApiSettings>(storeScope);
            var model = apiSettings.ToModel();

            // Store Settings
            model.ActiveStoreScopeConfiguration = storeScope;

            if (model.EnableApi_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(apiSettings, x => x.EnableApi, storeScope, false);
            }

            //now clear settings cache
            _settingService.ClearCache();

            return View($"~/Plugins/Nop.Plugin.Api/Areas/Admin/Views/ApiAdmin/Settings.cshtml", model);
        }

        [HttpPost]
        public IActionResult Settings(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;

            var settings = model.ToEntity();

            /* We do not clear cache after each setting update.
            * This behavior can increase performance because cached settings will not be cleared 
            * and loaded from database after each update */

            if (model.EnableApi_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(settings, x => x.EnableApi, storeScope, false);
            }

            //now clear settings cache
            _settingService.ClearCache();

            _customerActivityService.InsertActivity("EditApiSettings", "Edit Api Settings");

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return View($"~/Plugins/Nop.Plugin.Api/Areas/Admin/Views/ApiAdmin/Settings.cshtml", model);
        }
    }
}
