﻿namespace Nop.Plugin.Api.Controllers.Admin
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Constants;
    using Nop.Services.Localization;
    using Web.Framework;
    using Nop.Web.Framework.Controllers;
    using Web.Framework.Kendoui;
    using Web.Framework.Mvc.Filters;
    using Models;
    using Services;
    using System;

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
            var gridModel = _clientService.GetAllClients();

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

                SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Created"));
                return continueEditing ? RedirectToAction("Edit", new { id = clientId }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("edit/{id:int}")]
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
              
                SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Edit"));
                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Delete")]
        [Route("delete/{id:int}")]
        public IActionResult DeleteConfirmed(int id)
        {
            _clientService.DeleteClient(id);

            SuccessNotification(_localizationService.GetResource("Plugins.Api.Admin.Client.Deleted"));
            return RedirectToAction("List");
        }
    }
}