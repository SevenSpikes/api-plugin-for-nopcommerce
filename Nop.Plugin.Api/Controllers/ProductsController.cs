using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
        private readonly IProductTagService _productTagService;
        private readonly IProductAttributeService _productAttributeService;

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
                                  IProductAttributeService productAttributeService) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _factory = factory;
            _pictureService = pictureService;
            _manufacturerService = manufacturerService;
            _productTagService = productTagService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _productAttributeService = productAttributeService;
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

                MapAdditionalPropertiesToDTO(product, productDto);

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
            MapAdditionalPropertiesToDTO(product, productDto);

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
            ProductDto productDto = product.ToDto();
            MapAdditionalPropertiesToDTO(product, productDto);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

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

            // We do not need to validate the product id, because this will happen in the model binder using the dto validator.
            int productId = int.Parse(productDelta.Dto.Id);

            Product product = _productApiService.GetProductById(productId);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            productDelta.Merge(product);

            product.UpdatedOnUtc = DateTime.UtcNow;
            _productService.UpdateProduct(product);

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
            ProductDto productDto = product.ToDto();
            MapAdditionalPropertiesToDTO(product, productDto);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

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

        private void MapAdditionalPropertiesToDTO(Product product, ProductDto productDto)
        {
            PrepareProductImages(product.ProductPictures, productDto);
            PrepareProductAttributes(product.ProductAttributeMappings, productDto);

            productDto.SeName = product.GetSeName();
            productDto.DiscountIds = product.AppliedDiscounts.Select(discount => discount.Id).ToList();
            productDto.ManufacturerIds = product.ProductManufacturers.Select(pm => pm.ManufacturerId).ToList();
            productDto.RoleIds = _aclService.GetAclRecords(product).Select(acl => acl.CustomerRoleId).ToList();
            productDto.StoreIds = _storeMappingService.GetStoreMappings(product).Select(mapping => mapping.StoreId).ToList();
            productDto.Tags = product.ProductTags.Select(tag => tag.Name).ToList();

            productDto.AssociatedProductIds =
                _productService.GetAssociatedProducts(product.Id, showHidden: true)
                    .Select(associatedProduct => associatedProduct.Id)
                    .ToList();
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

        private void PrepareProductImages(IEnumerable<ProductPicture> productPictures, ProductDto productDto)
        {
            if (productDto.Images == null)
                productDto.Images = new List<ImageMappingDto>();

            // Here we prepare the resulted dto image.
            foreach (var productPicture in productPictures)
            {
                ImageDto imageDto = PrepareImageDto(productPicture.Picture);

                if (imageDto != null)
                {
                    ImageMappingDto productImageDto = new ImageMappingDto();
                    productImageDto.Id = productPicture.Id;
                    productImageDto.Position = productPicture.DisplayOrder;
                    productImageDto.Src = imageDto.Src;
                    productImageDto.Attachment = imageDto.Attachment;

                    productDto.Images.Add(productImageDto);
                }
            }
        }

        private void PrepareProductAttributes(IEnumerable<ProductAttributeMapping> productAttributeMappings, ProductDto productDto)
        {
            if (productDto.ProductAttributeMappings == null)
                productDto.ProductAttributeMappings = new List<ProductAttributeMappingDto>();

            foreach (var productAttributeMapping in productAttributeMappings)
            {
                ProductAttributeMappingDto productAttributeMappingDto = PrepareProductAttributeMappingDto(productAttributeMapping);

                if (productAttributeMappingDto != null)
                {
                    productDto.ProductAttributeMappings.Add(productAttributeMappingDto);
                }
            }
        }

        private ProductAttributeMappingDto PrepareProductAttributeMappingDto(ProductAttributeMapping productAttributeMapping)
        {
            ProductAttributeMappingDto productAttributeMappingDto = null;

            if (productAttributeMapping != null)
            {
                productAttributeMappingDto = new ProductAttributeMappingDto()
                {
                    Id = productAttributeMapping.Id,
                    ProductAttributeId = productAttributeMapping.ProductAttributeId,
                    ProductAttributeName = _productAttributeService.GetProductAttributeById(productAttributeMapping.ProductAttributeId).Name,
                    TextPrompt = productAttributeMapping.TextPrompt,
                    DefaultValue = productAttributeMapping.DefaultValue,
                    AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                    DisplayOrder = productAttributeMapping.DisplayOrder,
                    IsRequired = productAttributeMapping.IsRequired,
                    ProductAttributeValues = productAttributeMapping.ProductAttributeValues.Select(x => PrepareProductAttributeValueDto(x,productAttributeMapping.Product)).ToList()
                };
            }

            return productAttributeMappingDto;
        }

        private ProductAttributeValueDto PrepareProductAttributeValueDto(ProductAttributeValue productAttributeValue,Product product)
        {
            ProductAttributeValueDto productAttributeValueDto = null;

            if (productAttributeValue != null)
            {
                productAttributeValueDto = productAttributeValue.ToDto();
                if (productAttributeValue.ImageSquaresPictureId > 0)
                {
                    Picture imageSquaresPicture = _pictureService.GetPictureById(productAttributeValue.ImageSquaresPictureId);
                    ImageDto imageDto = PrepareImageDto(imageSquaresPicture);
                    productAttributeValueDto.ImageSquaresImage = imageDto;
                }

                if (productAttributeValue.PictureId > 0)
                {
                    // make sure that the picture is mapped to the product
                    // This is needed since if you delete the product picture mapping from the nopCommerce administrationthe
                    // then the attribute value is not updated and it will point to a picture that has been deleted
                   var productPicture = product.ProductPictures.FirstOrDefault(pp => pp.PictureId == productAttributeValue.PictureId);
                    if (productPicture != null)
                    {
                        productAttributeValueDto.ProductPictureId = productPicture.Id;
                    }
                }
            }

            return productAttributeValueDto;
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