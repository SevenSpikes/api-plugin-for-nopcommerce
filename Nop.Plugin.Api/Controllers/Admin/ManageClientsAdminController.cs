using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Controllers;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models;
using Nop.Plugin.Api.Services;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Api.Controllers.Admin
{
    [AdminAuthorize]
    public class ManageClientsAdminController : BaseAdminController
    {
        private readonly IClientService _clientService;
        private readonly ILocalizationService _localizationService;

        public ManageClientsAdminController(IClientService clientService,
            ILocalizationService localizationService)
        {
            _clientService = clientService;
            _localizationService = localizationService;
        }

        [HttpGet]
        public ActionResult List()
        {
            return View(ViewNames.AdminApiClientsList);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            IList<ClientModel> gridModel = PrepareListModel();

            var grids = new DataSourceResult()
            {
                Data = gridModel,
                Total = gridModel.Count()
            };

            return Json(grids);
        }

        public ActionResult Create()
        {
            ClientModel clientModel = PrepareClientModel();

            return View(ViewNames.AdminApiClientsCreate, clientModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(ClientModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                Client client = model.ToEntity();

                _clientService.InsertClient(client);

                SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Created"));
                return continueEditing ? RedirectToAction("Edit", new { id = client.Id }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            Client client = _clientService.GetClientById(id);

            var clientModel = new ClientModel();

            if (client != null)
            {
                clientModel = client.ToModel();
            }

            return View(ViewNames.AdminApiClientsEdit, clientModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(ClientModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                Client editedClient = _clientService.GetClientById(model.Id);

                editedClient = model.ToEntity(editedClient);

                _clientService.UpdateClient(editedClient);

                SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Edit"));
                return continueEditing ? RedirectToAction("Edit", new { id = editedClient.Id }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        public ActionResult DeleteClient(int id, DataSourceRequest command)
        {
            Client client = _clientService.GetClientById(id);
            if (client == null)
                throw new ArgumentException("No client found with the specified id");

            _clientService.DeleteClient(client);

            return List(command);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = _clientService.GetClientById(id);
            _clientService.DeleteClient(client);

            SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Deleted"));
            return RedirectToAction("List");
        }
        
        private IList<ClientModel> PrepareListModel()
        {
            IList<Client> clients = _clientService.GetAllClients();

            var clientModels = new List<ClientModel>();

            foreach (var client in clients)
            {
                clientModels.Add(client.ToModel());
            }

            return clientModels;
        }

        private ClientModel PrepareClientModel()
        {
            var clientModel = new ClientModel()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientSecret = Guid.NewGuid().ToString(),
                IsActive = true
            };

            return clientModel;
        }
    }
}