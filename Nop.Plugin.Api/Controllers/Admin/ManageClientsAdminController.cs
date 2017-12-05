namespace Nop.Plugin.Api.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Nop.Plugin.Api.Constants;
    using Nop.Services.Localization;
    using Nop.Web.Framework;
    using Nop.Web.Framework.Controllers;
    using Nop.Web.Framework.Kendoui;
    using Nop.Web.Framework.Mvc.Filters;
    using Nop.Plugin.Api.Models;
    using Nop.Plugin.Api.Services;

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [Route("admin/manageClientsAdmin/")]
    public class ManageClientsAdminController : BasePluginController
    {
        private readonly IClientService _clientService;
        private readonly ILocalizationService _localizationService;

        public ManageClientsAdminController(ILocalizationService localizationService, IClientService clientService)
        {
            _localizationService = localizationService;
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
        public ActionResult List(DataSourceRequest command)
        {
            IList<ClientApiModel> gridModel = _clientService.GetAllClients();

            var grids = new DataSourceResult()
            {
                Data = gridModel,
                Total = gridModel.Count()
            };

            return Json(grids);
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            ClientApiModel clientModel = PrepareClientModel();

            return View(ViewNames.AdminApiClientsCreate, clientModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("create")]
        public ActionResult Create(ClientApiModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                _clientService.InsertClient(model);

                SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Created"));
                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            ClientApiModel clientModel = _clientService.FindClientByIdAsync(id);
            
            return View(ViewNames.AdminApiClientsEdit, clientModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(ClientApiModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                _clientService.UpdateClient(model);
              
                SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Edit"));
                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete/{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            _clientService.DeleteClient(id);

            SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Deleted"));
            return RedirectToAction("List");
        }

        private ClientApiModel PrepareClientModel()
        {
            string clientSecretRaw = Guid.NewGuid().ToString();

            var clientModel = new ClientApiModel()
            {
                ClientId = Guid.NewGuid().ToString(),
                Enabled = true,
                RedirectUrl = string.Empty,
                ClientName = string.Empty,
                ClientSecretDescription = clientSecretRaw
            };

            return clientModel;
        }
    }
}