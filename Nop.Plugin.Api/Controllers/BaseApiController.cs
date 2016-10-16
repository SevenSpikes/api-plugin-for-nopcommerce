using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly IJsonFieldsSerializer _jsonFieldsSerializer;
        protected readonly IAclService _aclService;
        protected readonly ICustomerService _customerService;
        protected readonly IStoreMappingService _storeMappingService;
        protected readonly IStoreService _storeService;
        protected readonly IDiscountService _discountService;
        protected readonly ICustomerActivityService _customerActivityService;
        protected readonly ILocalizationService _localizationService;

        public BaseApiController(IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService, 
            IStoreService storeService, 
            IDiscountService discountService, 
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService)
        {
            _jsonFieldsSerializer = jsonFieldsSerializer;
            _aclService = aclService;
            _customerService = customerService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _discountService = discountService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
        }

        protected IHttpActionResult Error(HttpStatusCode statusCode = (HttpStatusCode)422, string propertyKey = "", string errorMessage = "")
        {
            var errors = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(propertyKey))
            {
                var errorsList = new List<string>() {errorMessage};
                errors.Add(propertyKey, errorsList);
            }
            
            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                List<string> validErrorMessages = new List<string>();

                if (errorMessages != null)
                {
                    validErrorMessages.AddRange(errorMessages.Where(message => !string.IsNullOrEmpty(message)));
                }

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

            var errorsRootObject = new ErrorsRootObject()
            {
                Errors = errors
            };

            var errorsJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);

            return new ErrorActionResult(errorsJson, statusCode);
        }

        protected List<int> UpdateAclRoles<TEntity>(TEntity entity, List<int> passedRoleIds) where TEntity: BaseEntity, IAclSupported
        {
            IList<AclRecord> existingAclRecords = _aclService.GetAclRecords(entity);

            var roleIds = new List<int>();

            if (passedRoleIds == null)
            {
                roleIds = existingAclRecords.Select(acl => acl.CustomerRoleId).ToList();
            }
            else
            {
                IList<CustomerRole> allCustomerRoles = _customerService.GetAllCustomerRoles(true);

                foreach (var customerRole in allCustomerRoles)
                {
                    if (passedRoleIds.Contains(customerRole.Id))
                    {
                        //new role
                        if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        {
                            _aclService.InsertAclRecord(entity, customerRole.Id);
                        }

                        roleIds.Add(customerRole.Id);
                    }
                    else
                    {
                        //remove role
                        var aclRecordToDelete =
                            existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);

                        if (aclRecordToDelete != null)
                        {
                            _aclService.DeleteAclRecord(aclRecordToDelete);
                            roleIds.Remove(customerRole.Id);
                        }
                    }
                }
            }

            entity.SubjectToAcl = roleIds.Count > 0;

            return roleIds;
        }

        protected List<int> UpdateStoreMappings<TEntity>(TEntity entity, List<int> passedStoreIds) where TEntity : BaseEntity, IStoreMappingSupported
        {
            IList<StoreMapping> existingStoreMappings = _storeMappingService.GetStoreMappings(entity);

            var storeIds = new List<int>();

            if (passedStoreIds == null)
            {
                storeIds = existingStoreMappings.Select(x => x.StoreId).ToList();
            }
            else
            {
                IList<Store> allStores = _storeService.GetAllStores();

                foreach (var store in allStores)
                {
                    if (passedStoreIds.Contains(store.Id))
                    {
                        //new store
                        if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        {
                            _storeMappingService.InsertStoreMapping(entity, store.Id);
                        }

                        storeIds.Add(store.Id);
                    }
                    else
                    {
                        //remove store
                        StoreMapping storeMappingToDelete =
                            existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);

                        if (storeMappingToDelete != null)
                        {
                            _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                            storeIds.Remove(store.Id);
                        }
                    }
                }
            }

            entity.LimitedToStores = storeIds.Count > 0;

            return storeIds;
        }

        protected List<int> ApplyDiscountsToEntity<TEntity>(TEntity entity, List<int> passedDiscountIds, DiscountType discountType) where TEntity : BaseEntity
        {
            // Because there is no interface to ensure that the applied discounts property will be present in the entity we are doing the below logic in trust.
            var discountIds = new List<int>();
            var appliedDiscountsProperty = entity.GetType().GetProperty("AppliedDiscounts").GetValue(entity) as IList<Discount>;

            if (appliedDiscountsProperty == null) return discountIds;

            if (passedDiscountIds == null)
            {
                discountIds = appliedDiscountsProperty.Select(discount => discount.Id).ToList();
            }
            else
            {
                Dictionary<int, bool> appliedDiscounts = appliedDiscountsProperty.ToDictionary(x => x.Id, x => true);

                // Ensure that there won't be repeating discounts.
                var uniqueDiscounts = new HashSet<int>(passedDiscountIds);

                var allDiscounts = _discountService.GetAllDiscounts(discountType, showHidden: true);

                foreach (var discount in allDiscounts)
                {
                    // Apply the discount. This is a failsafe to make sure that you won't apply the discount two times.
                    if (!appliedDiscounts.ContainsKey(discount.Id) && uniqueDiscounts.Contains(discount.Id))
                    {
                        appliedDiscountsProperty.Add(discount);
                        discountIds.Add(discount.Id);
                    }
                    // Remove the discount
                    else if (appliedDiscounts.ContainsKey(discount.Id) && !uniqueDiscounts.Contains(discount.Id))
                    {
                        appliedDiscountsProperty.Remove(discount);
                        discountIds.Remove(discount.Id);
                    }
                    // Here we make sure that we will add the discount if it is already applied and it is not for removal.
                    else if (appliedDiscounts.ContainsKey(discount.Id))
                    {
                        discountIds.Add(discount.Id);
                    }
                }
            }

            return discountIds;
        }

        protected ImageDto PrepareImageDto<TDto>(Picture picture, TDto dto)
        {
            ImageDto image = null;

            if (picture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                image = new ImageDto()
                {
                    Attachment = Convert.ToBase64String(picture.PictureBinary)
                };
            }

            return image;
        }
    }
}