﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nop.Core;
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
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ProductsController : BaseApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IFactory<Product> _factory;
        private readonly IProductTagService _productTagService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDTOHelper _dtoHelper;

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
                                  IManufacturerService manufacturerService,
                                  IProductTagService productTagService,
                                  IProductAttributeService productAttributeService,
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _factory = factory;
            _manufacturerService = manufacturerService;
            _productTagService = productTagService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _dtoHelper = dtoHelper;
            _pictureService = pictureService;
        }

        /// <summary>
        /// Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProducts(ProductsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var allProducts = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                        parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId,
                                                                        parameters.VendorName, parameters.PublishedStatus)
                                                .Where(p => _storeMappingService.Authorize(p));
            
            IList<ProductDto> productsAsDtos = allProducts.Select(product =>
            {
                return _dtoHelper.PrepareProductDTO(product);

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
        [Route("/api/products/count")]
        [ProducesResponseType(typeof(ProductsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductsCount(ProductsCountParametersModel parameters)
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
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductById(int id, string fields = "")
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

            ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), 422)]
        public IActionResult CreateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            Product product = _factory.Initialize();
            productDelta.Merge(product);

            _productService.InsertProduct(product);

            UpdateProductPictures(product, productDelta.Dto.Images);

            UpdateProductTags(product, productDelta.Dto.Tags);

            UpdateProductManufacturers(product, productDelta.Dto.ManufacturerIds);

            UpdateAssociatedProducts(product, productDelta.Dto.AssociatedProductIds);

            //search engine name
            var seName = product.ValidateSeName(productDelta.Dto.SeName, product.Name, true);
            _urlRecordService.SaveSlug(product, seName, 0);

            UpdateAclRoles(product, productDelta.Dto.RoleIds);

            UpdateDiscountMappings(product, productDelta.Dto.DiscountIds);

            UpdateStoreMappings(product, productDelta.Dto.StoreIds);

            _productService.UpdateProduct(product);

            _customerActivityService.InsertActivity("AddNewProduct",
                _localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name);

            // Preparing the result dto of the new product
            ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), 422)]
        public IActionResult UpdateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the product id, because this will happen in the model binder using the dto validator.
            int productId = int.Parse(productDelta.Dto.Id);

            Product product = _productApiService.GetProductById(productId);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            productDelta.Merge(product,true);

            product.UpdatedOnUtc = DateTime.UtcNow;
            _productService.UpdateProduct(product);

            UpdateProductAttributes(product, productDelta);

            UpdateProductPictures(product, productDelta.Dto.Images);

            UpdateProductTags(product, productDelta.Dto.Tags);

            UpdateProductManufacturers(product, productDelta.Dto.ManufacturerIds);

            UpdateAssociatedProducts(product, productDelta.Dto.AssociatedProductIds);

            // Update the SeName if specified
            if (productDelta.Dto.SeName != null)
            {
                var seName = product.ValidateSeName(productDelta.Dto.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, seName, 0);
            }

            UpdateDiscountMappings(product, productDelta.Dto.DiscountIds);

            UpdateStoreMappings(product, productDelta.Dto.StoreIds);

            UpdateAclRoles(product, productDelta.Dto.RoleIds);

            _productService.UpdateProduct(product);

            _customerActivityService.InsertActivity("UpdateProduct",
               _localizationService.GetResource("ActivityLog.UpdateProduct"), product.Name);

            // Preparing the result dto of the new product
            ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteProduct(int id)
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

        private void UpdateProductPictures(Product entityToUpdate, List<ImageMappingDto> setPictures)
        {
            // If no pictures are specified means we don't have to update anything
            if (setPictures == null)
                return;

            // delete unused product pictures
            var unusedProductPictures = entityToUpdate.ProductPictures.Where(x => setPictures.All(y => y.Id != x.Id)).ToList();
            foreach (var unusedProductPicture in unusedProductPictures)
            {
                var picture = _pictureService.GetPictureById(unusedProductPicture.PictureId);
                if (picture == null)
                    throw new ArgumentException("No picture found with the specified id");
                _pictureService.DeletePicture(picture);
            }

            foreach (var imageDto in setPictures)
            {
                if (imageDto.Id > 0)
                {
                    // update existing product picture
                    var productPictureToUpdate = entityToUpdate.ProductPictures.FirstOrDefault(x => x.Id == imageDto.Id);
                    if (productPictureToUpdate != null && imageDto.Position > 0)
                    {
                        productPictureToUpdate.DisplayOrder = imageDto.Position;
                        _productService.UpdateProductPicture(productPictureToUpdate);
                    }
                }
                else
                {
                    // add new product picture
                    Picture newPicture = _pictureService.InsertPicture(imageDto.Binary, imageDto.MimeType, string.Empty);
                    _productService.InsertProductPicture(new ProductPicture()
                    {
                        PictureId = newPicture.Id,
                        ProductId = entityToUpdate.Id,
                        DisplayOrder = imageDto.Position
                    });
                }
            }
        }

        private void UpdateProductAttributes(Product entityToUpdate, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute mappings are specified means we don't have to update anything
            if (productDtoDelta.Dto.ProductAttributeMappings == null)
                return;

            // delete unused product attribute mappings
            IEnumerable<int> toBeUpdatedIds = productDtoDelta.Dto.ProductAttributeMappings.Where(y => y.Id != 0).Select(x => x.Id);

            var unusedProductAttributeMappings = entityToUpdate.ProductAttributeMappings.Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeMapping in unusedProductAttributeMappings)
            {
                _productAttributeService.DeleteProductAttributeMapping(unusedProductAttributeMapping);
            }

            foreach (var productAttributeMappingDto in productDtoDelta.Dto.ProductAttributeMappings)
            {
                if (productAttributeMappingDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeMappingToUpdate = entityToUpdate.ProductAttributeMappings.FirstOrDefault(x => x.Id == productAttributeMappingDto.Id);
                    if (productAttributeMappingToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeMappingDto,productAttributeMappingToUpdate,false);
                       
                        _productAttributeService.UpdateProductAttributeMapping(productAttributeMappingToUpdate);

                        UpdateProductAttributeValues(productAttributeMappingDto, productDtoDelta);
                    }
                }
                else
                {
                    ProductAttributeMapping newProductAttributeMapping = new ProductAttributeMapping();
                    newProductAttributeMapping.ProductId = entityToUpdate.Id;

                    productDtoDelta.Merge(productAttributeMappingDto, newProductAttributeMapping);

                    // add new product attribute
                    _productAttributeService.InsertProductAttributeMapping(newProductAttributeMapping);
                }
            }
        }

        private void UpdateProductAttributeValues(ProductAttributeMappingDto productAttributeMappingDto, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute values are specified means we don't have to update anything
            if (productAttributeMappingDto.ProductAttributeValues == null)
                return;

            // delete unused product attribute values
            IEnumerable<int> toBeUpdatedIds = productAttributeMappingDto.ProductAttributeValues.Where(y => y.Id != 0).Select(x => x.Id);

            var unusedProductAttributeValues =
                _productAttributeService.GetProductAttributeValues(productAttributeMappingDto.Id).Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList(); ;

            foreach (var unusedProductAttributeValue in unusedProductAttributeValues)
            {
                _productAttributeService.DeleteProductAttributeValue(unusedProductAttributeValue);
            }

            foreach (var productAttributeValueDto in productAttributeMappingDto.ProductAttributeValues)
            {
                if (productAttributeValueDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeValueToUpdate =
                        _productAttributeService.GetProductAttributeValueById(productAttributeValueDto.Id);
                    if (productAttributeValueToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeValueDto, productAttributeValueToUpdate, false);

                        _productAttributeService.UpdateProductAttributeValue(productAttributeValueToUpdate);
                    }
                }
                else
                {
                    ProductAttributeValue newProductAttributeValue = new ProductAttributeValue();
                    productDtoDelta.Merge(productAttributeValueDto, newProductAttributeValue);

                    newProductAttributeValue.ProductAttributeMappingId = productAttributeMappingDto.Id;
                    // add new product attribute value
                    _productAttributeService.InsertProductAttributeValue(newProductAttributeValue);
                }
            }
        }

        private void UpdateProductTags(Product product, List<string> productTags)
        {
            if (productTags == null)
                return;

            if (product == null)
                throw new ArgumentNullException("product");

            //product tags
            var existingProductTags = product.ProductTags.ToList();
            var productTagsToRemove = new List<ProductTag>();
            foreach (var existingProductTag in existingProductTags)
            {
                bool found = false;
                foreach (string newProductTag in productTags)
                {
                    if (existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    productTagsToRemove.Add(existingProductTag);
                }
            }
            foreach (var productTag in productTagsToRemove)
            {
                product.ProductTags.Remove(productTag);
                _productService.UpdateProduct(product);
            }
            foreach (string productTagName in productTags)
            {
                ProductTag productTag;
                var productTag2 = _productTagService.GetProductTagByName(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag
                    {
                        Name = productTagName
                    };
                    _productTagService.InsertProductTag(productTag);
                }
                else
                {
                    productTag = productTag2;
                }
                if (!product.ProductTagExists(productTag.Id))
                {
                    product.ProductTags.Add(productTag);
                    _productService.UpdateProduct(product);
                }
            }
        }

        private void UpdateDiscountMappings(Product product, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
                return;

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);

            foreach (var discount in allDiscounts)
            {
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                        product.AppliedDiscounts.Add(discount);
                }
                else
                {
                    //remove discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                        product.AppliedDiscounts.Remove(discount);
                }
            }

            _productService.UpdateProduct(product);
            _productService.UpdateHasDiscountsApplied(product);
        }
        
        private void UpdateProductManufacturers(Product product, List<int> passedManufacturerIds)
        {
            // If no manufacturers specified then there is nothing to map 
            if (passedManufacturerIds == null)
                return;

            var unusedProductManufacturers = product.ProductManufacturers.Where(x => !passedManufacturerIds.Contains(x.ManufacturerId)).ToList();

            // remove all manufacturers that are not passed
            foreach (var unusedProductManufacturer in unusedProductManufacturers)
            {
                _manufacturerService.DeleteProductManufacturer(unusedProductManufacturer);
            }

            foreach (var passedManufacturerId in passedManufacturerIds)
            {
                // not part of existing manufacturers so we will create a new one
                if (product.ProductManufacturers.All(x => x.ManufacturerId != passedManufacturerId))
                {
                    // if manufacturer does not exist we simply ignore it, otherwise add it to the product
                    var manufacturer = _manufacturerService.GetManufacturerById(passedManufacturerId);
                    if (manufacturer != null)
                    {
                        _manufacturerService.InsertProductManufacturer(new ProductManufacturer()
                        { ProductId = product.Id, ManufacturerId = manufacturer.Id });
                    }
                }
            }
        }

        private void UpdateAssociatedProducts(Product product, List<int> passedAssociatedProductIds)
        {
            // If no associated products specified then there is nothing to map 
            if (passedAssociatedProductIds == null)
                return;

            var noLongerAssociatedProducts =
                _productService.GetAssociatedProducts(product.Id, showHidden: true)
                    .Where(p => !passedAssociatedProductIds.Contains(p.Id));

            // update all products that are no longer associated with our product
            foreach (var noLongerAssocuatedProduct in noLongerAssociatedProducts)
            {
                noLongerAssocuatedProduct.ParentGroupedProductId = 0;
                _productService.UpdateProduct(noLongerAssocuatedProduct);
            }

            var newAssociatedProducts = _productService.GetProductsByIds(passedAssociatedProductIds.ToArray());
            foreach (var newAssociatedProduct in newAssociatedProducts)
            {
                newAssociatedProduct.ParentGroupedProductId = product.Id;
                _productService.UpdateProduct(newAssociatedProduct);
            }
        }
    }
}