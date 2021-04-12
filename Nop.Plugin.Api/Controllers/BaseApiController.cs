﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Api.Controllers
{
    [Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class BaseApiController : Controller
    {
        protected readonly IAclService AclService;
        protected readonly ICustomerActivityService CustomerActivityService;
        protected readonly ICustomerService CustomerService;
        protected readonly IDiscountService DiscountService;
        protected readonly IJsonFieldsSerializer JsonFieldsSerializer;
        protected readonly ILocalizationService LocalizationService;
        protected readonly IPictureService PictureService;
        protected readonly IStoreMappingService StoreMappingService;
        protected readonly IStoreService StoreService;

        public BaseApiController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService)
        {
            JsonFieldsSerializer = jsonFieldsSerializer;
            AclService = aclService;
            CustomerService = customerService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            DiscountService = discountService;
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            PictureService = pictureService;
        }

        [Route("/api/error/{statusCode}/{errorMessage}")]
        public IActionResult Error(HttpStatusCode statusCode = (HttpStatusCode)422, string propertyKey = "", string errorMessage = "")
        {
            var errors = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(propertyKey))
            {
                var errorsList = new List<string>
                                 {
                                     errorMessage
                                 };
                errors.Add(propertyKey, errorsList);
            }

            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                var validErrorMessages = new List<string>();

                validErrorMessages.AddRange(errorMessages.Where(message => !string.IsNullOrEmpty(message)));

                if (validErrorMessages.Count > 0)
                {
                    if (errors.ContainsKey(item.Key))
                    {
                        errors[item.Key].AddRange(validErrorMessages);
                    }
                    else
                    {
                        errors.Add(item.Key, validErrorMessages.ToList());
                    }
                }
            }

            var errorsRootObject = new ErrorsRootObject
            {
                Errors = errors
            };

            var errorsJson = JsonFieldsSerializer.Serialize(errorsRootObject, null);

            return new ErrorActionResult(errorsJson, statusCode);
        }

        protected void UpdateAclRoles<TEntity>(TEntity entity, List<int> passedRoleIds) where TEntity : BaseEntity, IAclSupported
        {
            if (passedRoleIds == null)
            {
                return;
            }

            entity.SubjectToAcl = passedRoleIds.Any();

            var existingAclRecords = AclService.GetAclRecords(entity);
            var allCustomerRoles = CustomerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (passedRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                    {
                        AclService.InsertAclRecord(entity, customerRole.Id);
                    }
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                    {
                        AclService.DeleteAclRecord(aclRecordToDelete);
                    }
                }
            }
        }

        protected void UpdateStoreMappings<TEntity>(TEntity entity, List<int> passedStoreIds) where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (passedStoreIds == null)
            {
                return;
            }

            entity.LimitedToStores = passedStoreIds.Any();

            var existingStoreMappings = StoreMappingService.GetStoreMappings(entity);
            var allStores = StoreService.GetAllStores();
            foreach (var store in allStores)
            {
                if (passedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                    {
                        StoreMappingService.InsertStoreMapping(entity, store.Id);
                    }
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                    {
                        StoreMappingService.DeleteStoreMapping(storeMappingToDelete);
                    }
                }
            }
        }
    }
}
