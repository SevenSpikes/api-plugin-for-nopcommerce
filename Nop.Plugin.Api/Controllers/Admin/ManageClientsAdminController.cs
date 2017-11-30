namespace Nop.Plugin.Api.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
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
        private readonly IClientStore _clientStore;
        private readonly IClientService _clientService;
        private readonly ILocalizationService _localizationService;

        public ManageClientsAdminController(IClientStore clientStore,
            ILocalizationService localizationService, IClientService clientService)
        {
            _clientStore = clientStore;
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
                return continueEditing ? RedirectToAction("Edit", new { clientId = model.ClientId }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("edit/{clientId}")]
        public async Task<IActionResult> Edit(string clientId)
        {
            var client = await _clientStore.FindClientByIdAsync(clientId);

            var clientModel = new ClientApiModel()
            {
                ClientId = client.ClientId,
                RedirectUrl = client.RedirectUris?.FirstOrDefault(),
                Enabled = client.Enabled,
                ClientName = client.ClientName,
                ClientSecretDescription = client.ClientSecrets?.FirstOrDefault()?.Description
            };
            
            return View(ViewNames.AdminApiClientsEdit, clientModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("edit/{clientId}")]
        public async Task<IActionResult> Edit(ClientApiModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                _clientService.UpdateClient(model);
              
                SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Edit"));
                return continueEditing ? RedirectToAction("Edit", new { clientId = model.ClientId }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [Route("delete/{clientId}")]
        public IActionResult DeleteClient(string clientId, DataSourceRequest command)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client Id must not be empty");

            _clientService.DeleteClient(clientId);

            return List(command);
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete/{clientId}")]
        public IActionResult DeleteConfirmed(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client Id must not be empty");

            _clientService.DeleteClient(clientId);

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