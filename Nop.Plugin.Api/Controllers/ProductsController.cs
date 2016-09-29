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
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ProductsController : BaseApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IFactory<Product> _factory;

        public ProductsController(IProductApiService productApiService, 
                                  IJsonFieldsSerializer jsonFieldsSerializer,
                                  IProductService productService, 
                                  IUrlRecordService urlRecordService, 
                                  ICustomerActivityService customerActivityService, 
                                  ILocalizationService localizationService,
                                  IFactory<Product> factory, 
                                  IAclService aclService, 
                                  IStoreMappingService storeMappingService, 
                                  IStoreService storeService, 
                                  ICustomerService customerService, 
                                  IDiscountService discountService, 
                                  IPictureService pictureService, 
                                  IManufacturerService manufacturerService) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService)
        {
            _productApiService = productApiService;
            _factory = factory;
            _pictureService = pictureService;
            _manufacturerService = manufacturerService;
            _urlRecordService = urlRecordService;
            _productService = productService;
        }

        /// <summary>
        /// Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ProductsRootObjectDto))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetProducts(ProductsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<Product> allProducts = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                        parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId,
                                                                        parameters.VendorName, parameters.PublishedStatus);

            IList<ProductDto> productsAsDtos = allProducts.Select(product =>
            {
                ProductDto productDto = product.ToDto();

                PrepareDtoAditionalProperties(product, productDto);

                return productDto;

            }).ToList();

            var productsRootObject = new ProductsRootObjectDto()
            {
                Products = productsAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ProductsCountRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetProductsCount(ProductsCountParametersModel parameters)
        {
            int allProductsCount = _productApiService.GetProductsCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                       parameters.UpdatedAtMax, parameters.PublishedStatus, parameters.VendorName, 
                                                                       parameters.CategoryId);

            var productsCountRootObject = new ProductsCountRootObject()
            {
                Count = allProductsCount
            };

            return Ok(productsCountRootObject);
        }

        /// <summary>
        /// Retrieve product by spcified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ProductsRootObjectDto))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetProductById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            Product product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }
            
            ProductDto productDto = product.ToDto();

            PrepareDtoAditionalProperties(product, productDto);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof(ProductsRootObjectDto))]
        public IHttpActionResult CreateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the productDelta object won't be null for sure so we don't need to check for this.

            // We need to insert the picture before the product so we can obtain the picture id and map it to the product.
            List<Picture> insertedPictures = InsertPicturesFromDtoInDatabase(productDelta.Dto.Images); 

            // Inserting the new product
            Product newProduct = _factory.Initialize();
            productDelta.Merge(newProduct);

            _productService.InsertProduct(newProduct);

            foreach (var picture in insertedPictures)
            {
                newProduct.ProductPictures.Add(new ProductPicture()
                {
                    PictureId = picture.Id,
                    ProductId = newProduct.Id
                    //TODO: display order
                });
            }

            MapTagsToProduct(newProduct, productDelta.Dto.Tags);

            _productService.UpdateProduct(newProduct);

            // We need to insert the entity first so we can have its id in order to map it to anything.
            // TODO: Localization

            List<int> manufacturerIds = null;

            if (productDelta.Dto.ManufacturerIds.Count > 0)
            {
                manufacturerIds = MapProductToManufacturers(newProduct.Id, productDelta.Dto.ManufacturerIds);
            }

            List<int> roleIds = null;

            if (productDelta.Dto.RoleIds.Count > 0)
            {
                roleIds = MapRoleToEntity(newProduct, productDelta.Dto.RoleIds);
            }

            List<int> discountIds = null;

            if (productDelta.Dto.DiscountIds.Count > 0)
            {
                discountIds = ApplyDiscountsToEntity(newProduct, productDelta.Dto.DiscountIds, DiscountType.AssignedToSkus);
                // Unable to add it to the method above (like it is implemented for the stores and roles) 
                // because the property is not part of any specific interface
                newProduct.HasDiscountsApplied = discountIds.Count > 0;
            }

            List<int> storeIds = null;

            if (productDelta.Dto.StoreIds.Count > 0)
            {
                storeIds = MapEntityToStores(newProduct, productDelta.Dto.StoreIds);
            }

            // Preparing the result dto of the new product
            ProductDto newProductDto = newProduct.ToDto();

            PrepareProductImages(insertedPictures, newProductDto);

            //search engine name
            newProductDto.SeName = newProduct.ValidateSeName(newProductDto.SeName, newProduct.Name, true);
            _urlRecordService.SaveSlug(newProduct, newProductDto.SeName, 0);

            if (manufacturerIds != null)
            {
                newProductDto.ManufacturerIds = manufacturerIds;
            }

            if (storeIds != null)
            {
                newProductDto.StoreIds = storeIds;
            }

            if (discountIds != null)
            {
                newProductDto.DiscountIds = discountIds;
            }

            if (roleIds != null)
            {
                newProductDto.RoleIds = roleIds;
            }

            _customerActivityService.InsertActivity("AddNewProduct",
                _localizationService.GetResource("ActivityLog.AddNewProduct"), newProduct.Name);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(newProductDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
        
        [HttpPut]
        [ResponseType(typeof(ProductsRootObjectDto))]
        public IHttpActionResult UpdateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the productDelta object won't be null for sure so we don't need to check for this.

            // We do not need to validate the product id, because this will happen in the model binder using the dto validator.
            int updateProductId = int.Parse(productDelta.Dto.Id);

            Product productEntityToUpdate = _productApiService.GetProductById(updateProductId);

            if (productEntityToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            productDelta.Merge(productEntityToUpdate);

            List<Picture> updatedPictures = UpdatePictures(productEntityToUpdate, productDelta.Dto.Images);

            MapTagsToProduct(productEntityToUpdate, productDelta.Dto.Tags);
            
            List<int> manufacturerIds = MapProductToManufacturers(productEntityToUpdate.Id, productDelta.Dto.ManufacturerIds);

            List<int> storeIds = MapEntityToStores(productEntityToUpdate, productDelta.Dto.StoreIds);

            List<int> roleIds = MapRoleToEntity(productEntityToUpdate, productDelta.Dto.RoleIds);

            List<int> discountIds = ApplyDiscountsToEntity(productEntityToUpdate, productDelta.Dto.DiscountIds, DiscountType.AssignedToSkus);
            productEntityToUpdate.HasDiscountsApplied = discountIds.Count > 0;

            productEntityToUpdate.UpdatedOnUtc = DateTime.UtcNow;

            _productService.UpdateProduct(productEntityToUpdate);

            _customerActivityService.InsertActivity("UpdateProduct",
               _localizationService.GetResource("ActivityLog.UpdateProduct"), productEntityToUpdate.Name);

            ProductDto newProductDto = productEntityToUpdate.ToDto();

            PrepareProductImages(updatedPictures, newProductDto);

            //search engine name
            newProductDto.SeName = productEntityToUpdate.ValidateSeName(newProductDto.SeName, productEntityToUpdate.Name, true);
            _urlRecordService.SaveSlug(productEntityToUpdate, newProductDto.SeName, 0);

            newProductDto.ManufacturerIds = manufacturerIds;
            newProductDto.StoreIds = storeIds;
            newProductDto.RoleIds = roleIds;
            newProductDto.DiscountIds = discountIds;

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(newProductDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            Product product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }
            
            _productService.DeleteProduct(product);

            //activity log
            _customerActivityService.InsertActivity("DeleteProduct", _localizationService.GetResource("ActivityLog.DeleteProduct"), product.Name);

            return new RawJsonActionResult("{}");
        }

        private void PrepareDtoAditionalProperties(Product product, ProductDto productDto)
        {
            List<Picture> productPictures = product.ProductPictures.Select(x => x.Picture).ToList();

            PrepareProductImages(productPictures, productDto);

            productDto.DiscountIds = product.AppliedDiscounts.Select(discount => discount.Id).ToList();
            productDto.RoleIds = _aclService.GetAclRecords(product).Select(acl => acl.CustomerRoleId).ToList();
            productDto.StoreIds = _storeMappingService.GetStoreMappings(product).Select(mapping => mapping.StoreId).ToList();
            productDto.Tags = product.ProductTags.Select(tag => tag.Name).ToList();
        }

        private List<Picture> UpdatePictures(Product entityToUpdate, List<ImageDto> setPictures)
        {
            List<Picture> productPictures = entityToUpdate.ProductPictures.Select(x => x.Picture).ToList();

            foreach (var productPicture in productPictures)
            {
                _pictureService.DeletePicture(productPicture);
            }

            List<Picture> updatedPictures = InsertPicturesFromDtoInDatabase(setPictures);

            foreach (var picture in updatedPictures)
            {
                entityToUpdate.ProductPictures.Add(new ProductPicture()
                {
                    ProductId = entityToUpdate.Id,
                    PictureId = picture.Id
                });
            }
            
            return updatedPictures;
        }

        private List<Picture> InsertPicturesFromDtoInDatabase(List<ImageDto> setPictures)
        {
            var insertedPictures = new List<Picture>();

            foreach (var image in setPictures)
            {
                Picture newPicture = _pictureService.InsertPicture(image.Binary, image.MimeType, string.Empty);

                insertedPictures.Add(newPicture);
            }

            return insertedPictures;
        }

        private void MapTagsToProduct(Product newProduct, List<string> passedTags)
        {
            Dictionary<string, ProductTag> currentProductTags = newProduct.ProductTags.ToDictionary(tag => tag.Name, tag => tag);
            var uniqueProductTagsToAdd = new HashSet<string>();

            foreach (var passedTag in passedTags)
            {
                // If tag already exists we remove it from the collection so we don't remove it later.
                // This will result in a collection in which, at the end, we will have a collection 
                // with all the tags that were mapped to the product, but currently are not part of the passed collection
                // which means we should delete product-tag mapping
                if (currentProductTags.ContainsKey(passedTag))
                {
                    currentProductTags.Remove(passedTag);
                }
                // new tag
                else
                {
                    uniqueProductTagsToAdd.Add(passedTag);
                }
            }

            // Delete all remaining tags.
            foreach (var productTag in currentProductTags)
            {
                newProduct.ProductTags.Remove(productTag.Value);
            }

            // Add tags that should be mapped to product.
            foreach (var productTag in uniqueProductTagsToAdd)
            {
                newProduct.ProductTags.Add(new ProductTag()
                {
                    Name = productTag
                });
            }
        }

        private void PrepareProductImages(List<Picture> pictures, ProductDto productDto)
        {
            // Here we prepare the resulted dto image.
            foreach (var insertedPicture in pictures)
            {
                ImageDto imageDto = PrepareImageDto(insertedPicture, productDto);

                if (imageDto != null)
                {
                    productDto.Images.Add(imageDto);
                }
            }
        }

        private List<int> MapProductToManufacturers(int entityId, List<int> passedManufacturerIds)
        {
            // Needed so we can easily check if an id is valid.
            Dictionary<int, bool> allManufacturers = _manufacturerService.GetAllManufacturers()
                .ToDictionary(manufacturer => manufacturer.Id, manufacturer => true);

            var mappedManufacturers = new List<int>();
            IList<ProductManufacturer> manufacturerMappingsForCurrentProduct = _manufacturerService.GetProductManufacturersByProductId(entityId);

            Dictionary<int, ProductManufacturer> existingMappings =
                manufacturerMappingsForCurrentProduct.ToDictionary(mapping => mapping.ManufacturerId, manufacturer => manufacturer);

            foreach (var manufacturerId in passedManufacturerIds)
            {
                if (!allManufacturers.ContainsKey(manufacturerId))
                {
                    // invalid manufacturer id so we just ignore it.
                    continue;
                }

                if (!existingMappings.ContainsKey(manufacturerId))
                {
                    // new mapping
                    var productManufacturer = new ProductManufacturer
                    {
                        ProductId = entityId,
                        ManufacturerId = manufacturerId
                    };

                    _manufacturerService.InsertProductManufacturer(productManufacturer);
                }
                else
                {
                    // Remove existing mapping to get the subset of all existing mappings that are not contained in the passed manufacturer ids.
                    existingMappings.Remove(manufacturerId);
                }

                mappedManufacturers.Add(manufacturerId);
            }

            // Remove all the existingMappings that have left. This will be used in the update to delete mappings.
            foreach (var mapping in existingMappings)
            {
                // This will delete the mapping not the manufacturer itself. The method name is little confusing.
                _manufacturerService.DeleteProductManufacturer(mapping.Value);
                mappedManufacturers.Remove(mapping.Key);
            }

            return mappedManufacturers;
        }
    }
}