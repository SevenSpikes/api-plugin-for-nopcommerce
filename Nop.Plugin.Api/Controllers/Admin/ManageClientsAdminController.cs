namespace Nop.Plugin.Api.Controllers.Admin
{
    using Constants;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Nop.Services.Localization;
    using Nop.Services.Messages;
    using Nop.Services.Security;
    using Nop.Web.Framework.Controllers;
    using Services;
    using System;
    using Web.Framework;
    using Web.Framework.Mvc.Filters;

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [Route("admin/manageClientsAdmin/")]
    public class ManageClientsAdminController : BasePluginController
    {
        private readonly IClientService _clientService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public ManageClientsAdminController(ILocalizationService localizationService, IClientService clientService, INotificationService notificationService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _clientService = clientService;
        }

        [HttpGet]
        [Route("list")]
        public ActionResult List()
        {
            return View(ViewNames.AdminApiClientsList);
        }

        [HttpPost]
        [Route("list")]
        public ActionResult List(ClientApiSearchModel searchModel)
        {
            var clients = _clientService.GetAllClients(searchModel);

            //throw new NotImplementedException();

            //var grids = new DataSourceResult()
            //{
            //    Data = gridModel,
            //    Total = gridModel.Count()
            //};

            return Json(clients);
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            var clientModel = new ClientApiModel
            {
                Enabled = true,
                ClientSecret = Guid.NewGuid().ToString(),
                ClientId = Guid.NewGuid().ToString(),
                AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration,
                RefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration
            };

            return View(ViewNames.AdminApiClientsCreate, clientModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("create")]
        public ActionResult Create(ClientApiModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var clientId = _clientService.InsertClient(model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Created"));
                return continueEditing ? RedirectToAction("Edit", new { id = clientId }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var clientModel = _clientService.FindClientByIdAsync(id);
            
            return View(ViewNames.AdminApiClientsEdit, clientModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("edit/{id}")]
        public IActionResult Edit(ClientApiModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                _clientService.UpdateClient(model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Edit"));
                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete/{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            _clientService.DeleteClient(id);

            _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Deleted"));
            return RedirectToAction("List");
        }
    }
}