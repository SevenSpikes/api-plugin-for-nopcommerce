using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly IFactory<Category> _factory; 

        public CategoriesController(ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            IFactory<Category> factory) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService)
        {
            _categoryApiService = categoryApiService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _factory = factory;
            _pictureService = pictureService;
        }

        /// <summary>
        /// Receive a list of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetCategories(CategoriesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            IList<Category> allCategories = _categoryApiService.GetCategories(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                             parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                             parameters.Limit, parameters.Page, parameters.SinceId,
                                                                             parameters.ProductId, parameters.PublishedStatus);

            IList<CategoryDto> categoriesAsDtos = allCategories.Select(category =>
            {
                CategoryDto categoryDto = category.ToDto();

                PrepareDtoAditionalProperties(category, categoryDto);

                return categoryDto;

            }).ToList();

            var categoriesRootObject = new CategoriesRootObject()
            {
                Categories = categoriesAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesCountRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            var allCategoriesCount = _categoryApiService.GetCategoriesCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                            parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                            parameters.PublishedStatus, parameters.ProductId);

            var categoriesCountRootObject = new CategoriesCountRootObject()
            {
                Count = allCategoriesCount
            };

            return Ok(categoriesCountRootObject);
        }

        /// <summary>
        /// Retrieve category by spcified id
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetCategoryById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            Category category = _categoryApiService.GetCategoryById(id);

            if (category == null)
            {
                return Error(HttpStatusCode.NotFound, "category", "category not found");
            }

            CategoryDto categoryDto = category.ToDto();

            PrepareDtoAditionalProperties(category, categoryDto);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(categoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }
        
        [HttpPost]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult CreateCategory([ModelBinder(typeof(JsonModelBinder<CategoryDto>))] Delta<CategoryDto> categoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the categoryDelta object won't be null for sure so we don't need to check for this.

            Picture insertedPicture = null;

            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            if (categoryDelta.Dto.Image.Binary != null)
            {
                insertedPicture = _pictureService.InsertPicture(categoryDelta.Dto.Image.Binary, categoryDelta.Dto.Image.MimeType, string.Empty);
            }

            // Inserting the new category
            Category newCategory = _factory.Initialize();
            categoryDelta.Merge(newCategory);

            if (insertedPicture != null)
            {
                newCategory.PictureId = insertedPicture.Id;
            }

            _categoryService.InsertCategory(newCategory);

            // We need to insert the entity first so we can have its id in order to map it to anything.
            // TODO: Localization
            List<int> roleIds = null;

            if (categoryDelta.Dto.RoleIds.Count > 0)
            {
                roleIds = UpdateAclRoles(newCategory, categoryDelta.Dto.RoleIds);
            }

            List<int> discountIds = null;

            if (categoryDelta.Dto.DiscountIds.Count > 0)
            {
                discountIds = ApplyDiscountsToEntity(newCategory, categoryDelta.Dto.DiscountIds, DiscountType.AssignedToCategories);
            }

            List<int> storeIds = null;

            if (categoryDelta.Dto.StoreIds.Count > 0)
            {
                storeIds = UpdateStoreMappings(newCategory, categoryDelta.Dto.StoreIds);
            }

            // Preparing the result dto of the new category
            CategoryDto newCategoryDto = newCategory.ToDto();

            //search engine name
            newCategoryDto.SeName = newCategory.ValidateSeName(newCategoryDto.SeName, newCategory.Name, true);
            _urlRecordService.SaveSlug(newCategory, newCategoryDto.SeName, 0);

            // Here we prepare the resulted dto image.
            ImageDto imageDto = PrepareImageDto(insertedPicture, newCategoryDto);

            if (imageDto != null)
            {
                newCategoryDto.Image = imageDto;
            }
            
            if (storeIds != null)
            {
                newCategoryDto.StoreIds = storeIds;
            }

            if (discountIds != null)
            {
                newCategoryDto.DiscountIds = discountIds;
            }

            if (roleIds != null)
            {
                newCategoryDto.RoleIds = roleIds;
            }

            _customerActivityService.InsertActivity("AddNewCategory",
                _localizationService.GetResource("ActivityLog.AddNewCategory"), newCategory.Name);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(newCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult UpdateCategory(
            [ModelBinder(typeof (JsonModelBinder<CategoryDto>))] Delta<CategoryDto> categoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the category id, because this will happen in the model binder using the dto validator.
            int updateCategoryId = int.Parse(categoryDelta.Dto.Id);

            Category categoryEntityToUpdate = _categoryApiService.GetCategoryById(updateCategoryId);

            if (categoryEntityToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "category", "category not found");
            }

            categoryDelta.Merge(categoryEntityToUpdate);
     
            Picture updatedPicture = UpdatePicture(categoryEntityToUpdate, categoryDelta.Dto.Image.Binary, categoryDelta.Dto.Image.MimeType);

            List<int> storeIds = UpdateStoreMappings(categoryEntityToUpdate, categoryDelta.Dto.StoreIds);
         
            List<int> roleIds = UpdateAclRoles(categoryEntityToUpdate, categoryDelta.Dto.RoleIds);

            List<int> discountIds = ApplyDiscountsToEntity(categoryEntityToUpdate, categoryDelta.Dto.DiscountIds, DiscountType.AssignedToCategories);

            categoryEntityToUpdate.UpdatedOnUtc = DateTime.UtcNow;

            _categoryService.UpdateCategory(categoryEntityToUpdate);

            _customerActivityService.InsertActivity("UpdateCategory",
                _localizationService.GetResource("ActivityLog.UpdateCategory"), categoryEntityToUpdate.Name);

            CategoryDto updatedCategoryDto = categoryEntityToUpdate.ToDto();

            PrepareImageDto(updatedPicture, updatedCategoryDto);
            
            updatedCategoryDto.StoreIds = storeIds;
            
            updatedCategoryDto.RoleIds = roleIds;

            updatedCategoryDto.DiscountIds = discountIds;

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(updatedCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            Category categoryToDelete = _categoryApiService.GetCategoryById(id);

            if (categoryToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "category", "category not found");
            }

            _categoryService.DeleteCategory(categoryToDelete);

            //activity log
            _customerActivityService.InsertActivity("DeleteCategory", _localizationService.GetResource("ActivityLog.DeleteCategory"), categoryToDelete.Name);

            return new RawJsonActionResult("{}");
        }

        private void PrepareDtoAditionalProperties(Category category, CategoryDto categoryDto)
        {
            Picture picture = _pictureService.GetPictureById(category.PictureId);
            ImageDto imageDto = PrepareImageDto(picture, categoryDto);

            if (imageDto != null)
            {
                categoryDto.Image = imageDto;
            }
            
            categoryDto.DiscountIds = category.AppliedDiscounts.Select(discount => discount.Id).ToList();
            categoryDto.RoleIds = _aclService.GetAclRecords(category).Select(acl => acl.CustomerRoleId).ToList();
            categoryDto.StoreIds = _storeMappingService.GetStoreMappings(category).Select(mapping => mapping.StoreId).ToList();
        }

        private Picture UpdatePicture(Category categoryEntityToUpdate, byte[] imageBytes, string mimeType)
        {
            Picture updatedPicture = null;
            Picture currentCategoryPicture = _pictureService.GetPictureById(categoryEntityToUpdate.PictureId);

            // when there is a picture set for the category
            if (currentCategoryPicture != null)
            {
                _pictureService.DeletePicture(currentCategoryPicture);

                // When the image attachment is null or empty.
                if (imageBytes == null)
                {
                    categoryEntityToUpdate.PictureId = 0;
                }
                else
                {
                    updatedPicture = _pictureService.InsertPicture(imageBytes, mimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
            // when there isn't a picture set for the category
            else
            {
                if (imageBytes != null)
                {
                    updatedPicture = _pictureService.InsertPicture(imageBytes, mimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }

            return updatedPicture;
        }
    }
}