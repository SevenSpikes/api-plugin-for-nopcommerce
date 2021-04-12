﻿using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.DTO.Categories;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.DTO.Languages;
using Nop.Plugin.Api.DTO.Manufacturers;
using Nop.Plugin.Api.DTO.OrderItems;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.DTO.ProductAttributes;
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.DTO.ShoppingCarts;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.DTO.Stores;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Services;
//using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Helpers
{
    public class DTOHelper : IDTOHelper
    {
        private readonly IAclService _aclService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ICustomerService _customerService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IDiscountService _discountService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IOrderService _orderService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productPictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

        public DTOHelper(
            IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            ICustomerService customerApiService,
            IProductAttributeParser productAttributeParser,
            ILanguageService languageService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IStoreService storeService,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IProductTagService productTagService,
            IDiscountService discountService,
            IManufacturerService manufacturerService,
            IOrderService orderService,
            IProductAttributeConverter productAttributeConverter,
            IShoppingCartService shoppingCartService)
        {
            _productService = productService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _customerService = customerApiService;
            _productAttributeParser = productAttributeParser;
            _languageService = languageService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _storeService = storeService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _productTagService = productTagService;
            _discountService = discountService;
            _manufacturerService = manufacturerService;
            _orderService = orderService;
            _productAttributeConverter = productAttributeConverter;
            _shoppingCartService = shoppingCartService;
        }

        public ProductDto PrepareProductDTO(Product product)
        {
            var productDto = product.ToDto();
            var productPictures = _productService.GetProductPicturesByProductId(product.Id);
            PrepareProductImages(productPictures, productDto);

            productDto.SeName = _urlRecordService.GetSeName(product);
            productDto.DiscountIds = _discountService.GetAppliedDiscounts(product).Select(discount => discount.Id).ToList();
            productDto.ManufacturerIds = _manufacturerService.GetProductManufacturersByProductId(product.Id).Select(pm => pm.Id).ToList();
            productDto.RoleIds = _aclService.GetAclRecords(product).Select(acl => acl.CustomerRoleId).ToList();
            productDto.StoreIds = _storeMappingService.GetStoreMappings(product).Select(mapping => mapping.StoreId)
                                                      .ToList();
            productDto.Tags = _productTagService.GetAllProductTagsByProductId(product.Id).Select(tag => tag.Name)
                                                .ToList();

            productDto.AssociatedProductIds =
                _productService.GetAssociatedProducts(product.Id, showHidden: true)
                               .Select(associatedProduct => associatedProduct.Id)
                               .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            productDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                                       {
                                           LanguageId = language.Id,
                                           LocalizedName = _localizationService.GetLocalized(product, x => x.Name, language.Id)
                                       };

                productDto.LocalizedNames.Add(localizedNameDto);
            }

            return productDto;
        }

        public CategoryDto PrepareCategoryDTO(Category category)
        {
            var categoryDto = category.ToDto();

            var picture = _pictureService.GetPictureById(category.PictureId);
            var imageDto = PrepareImageDto(picture);

            if (imageDto != null)
            {
                categoryDto.Image = imageDto;
            }

            categoryDto.SeName = _urlRecordService.GetSeName(category);
            categoryDto.DiscountIds = _discountService.GetAppliedDiscounts(category).Select(discount => discount.Id).ToList();
            categoryDto.RoleIds = _aclService.GetAclRecords(category).Select(acl => acl.CustomerRoleId).ToList();
            categoryDto.StoreIds = _storeMappingService.GetStoreMappings(category).Select(mapping => mapping.StoreId)
                                                       .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            categoryDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                                       {
                                           LanguageId = language.Id,
                                           LocalizedName = _localizationService.GetLocalized(category, x => x.Name, language.Id)
                                       };

                categoryDto.LocalizedNames.Add(localizedNameDto);
            }

            return categoryDto;
        }

        public OrderDto PrepareOrderDTO(Order order)
        {
            var orderDto = order.ToDto();

            orderDto.OrderItems = _orderService.GetOrderItems(order.Id).Select(PrepareOrderItemDTO).ToList();

            var customerDto = _customerService.GetCustomerById(order.CustomerId);

            if (customerDto != null)
            {
                orderDto.Customer = customerDto.ToOrderCustomerDto();
            }

            return orderDto;
        }

        public ShoppingCartItemDto PrepareShoppingCartItemDTO(ShoppingCartItem shoppingCartItem)
        {
            var dto = shoppingCartItem.ToDto();
            dto.ProductDto = PrepareProductDTO(_productService.GetProductById(shoppingCartItem.ProductId));
            dto.CustomerDto = _customerService.GetCustomerById(shoppingCartItem.CustomerId).ToCustomerForShoppingCartItemDto();
            dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
            return dto;
        }

        public OrderItemDto PrepareOrderItemDTO(OrderItem orderItem)
        {
            var dto = orderItem.ToDto();
            dto.Product = PrepareProductDTO(_productService.GetProductById(orderItem.ProductId));
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }

        public StoreDto PrepareStoreDTO(Store store)
        {
            var storeDto = store.ToDto();

            var primaryCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            if (!string.IsNullOrEmpty(primaryCurrency.DisplayLocale))
            {
                storeDto.PrimaryCurrencyDisplayLocale = primaryCurrency.DisplayLocale;
            }

            storeDto.LanguageIds = _languageService.GetAllLanguages(false, store.Id).Select(x => x.Id).ToList();

            return storeDto;
        }

        public LanguageDto PrepareLanguageDto(Language language)
        {
            var languageDto = language.ToDto();

            languageDto.StoreIds = _storeMappingService.GetStoreMappings(language).Select(mapping => mapping.StoreId)
                                                       .ToList();

            if (languageDto.StoreIds.Count == 0)
            {
                languageDto.StoreIds = _storeService.GetAllStores().Select(s => s.Id).ToList();
            }

            return languageDto;
        }

        public ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute)
        {
            return productAttribute.ToDto();
        }

        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute)
        {
            return productSpecificationAttribute.ToDto();
        }

        public SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute)
        {
            return specificationAttribute.ToDto();
        }

        public ManufacturerDto PrepareManufacturerDto(Manufacturer manufacturer)
        {
            var manufacturerDto = manufacturer.ToDto();

            var picture = _pictureService.GetPictureById(manufacturer.PictureId);
            var imageDto = PrepareImageDto(picture);

            if (imageDto != null)
            {
                manufacturerDto.Image = imageDto;
            }

            manufacturerDto.SeName = _urlRecordService.GetSeName(manufacturer);
            manufacturerDto.DiscountIds = _discountService.GetAppliedDiscounts(manufacturer).Select(discount => discount.Id).ToList();
            manufacturerDto.RoleIds = _aclService.GetAclRecords(manufacturer).Select(acl => acl.CustomerRoleId).ToList();
            manufacturerDto.StoreIds = _storeMappingService.GetStoreMappings(manufacturer).Select(mapping => mapping.StoreId)
                                                           .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            manufacturerDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                                       {
                                           LanguageId = language.Id,
                                           LocalizedName = _localizationService.GetLocalized(manufacturer, x => x.Name, language.Id)
                                       };

                manufacturerDto.LocalizedNames.Add(localizedNameDto);
            }

            return manufacturerDto;
        }

        private void PrepareProductImages(IEnumerable<ProductPicture> productPictures, ProductDto productDto)
        {
            if (productDto.Images == null)
            {
                productDto.Images = new List<ImageMappingDto>();
            }

            // Here we prepare the resulted dto image.
            foreach (var productPicture in productPictures)
            {
                var imageDto = PrepareImageDto(_pictureService.GetPictureById(productPicture.PictureId));

                if (imageDto != null)
                {
                    var productImageDto = new ImageMappingDto
                                          {
                                              Id = productPicture.Id,
                                              PictureId = productPicture.PictureId,
                                              Position = productPicture.DisplayOrder,
                                              Src = imageDto.Src,
                                              Attachment = imageDto.Attachment
                                          };

                    productDto.Images.Add(productImageDto);
                }
            }
        }

        protected ImageDto PrepareImageDto(Picture picture)
        {
            ImageDto image = null;

            if (picture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                image = new ImageDto
                        {
                            //Attachment = Convert.ToBase64String(picture.PictureBinary),
                            //Src = _pictureService.GetPictureUrl(picture)
                        };
            }

            return image;
        }

        private void PrepareProductAttributes(
            IEnumerable<ProductAttributeMapping> productAttributeMappings,
            ProductDto productDto)
        {
            if (productDto.ProductAttributeMappings == null)
            {
                productDto.ProductAttributeMappings = new List<ProductAttributeMappingDto>();
            }

            foreach (var productAttributeMapping in productAttributeMappings)
            {
                var productAttributeMappingDto =
                    PrepareProductAttributeMappingDto(productAttributeMapping);

                if (productAttributeMappingDto != null)
                {
                    productDto.ProductAttributeMappings.Add(productAttributeMappingDto);
                }
            }
        }

        private ProductAttributeMappingDto PrepareProductAttributeMappingDto(
            ProductAttributeMapping productAttributeMapping)
        {
            ProductAttributeMappingDto productAttributeMappingDto = null;

            if (productAttributeMapping != null)
            {
                productAttributeMappingDto = new ProductAttributeMappingDto
                                             {
                                                 Id = productAttributeMapping.Id,
                                                 ProductAttributeId = productAttributeMapping.ProductAttributeId,
                                                 ProductAttributeName = _productAttributeService
                                                                        .GetProductAttributeById(productAttributeMapping.ProductAttributeId).Name,
                                                 TextPrompt = productAttributeMapping.TextPrompt,
                                                 DefaultValue = productAttributeMapping.DefaultValue,
                                                 AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                                                 DisplayOrder = productAttributeMapping.DisplayOrder,
                                                 IsRequired = productAttributeMapping.IsRequired,
                                                 //TODO: Somnath
                                                 //ProductAttributeValues = _productAttributeService.GetProductAttributeValueById(productAttributeMapping.Id).
                                                 //                                                .Select(x =>
                                                 //                                                            PrepareProductAttributeValueDto(x,
                                                 //                                                                                            productAttributeMapping
                                                 //                                                                                                .Product))
                                                 //                                                .ToList()
                                             };
            }

            return productAttributeMappingDto;
        }

        private ProductAttributeValueDto PrepareProductAttributeValueDto(
            ProductAttributeValue productAttributeValue,
            Product product)
        {
            ProductAttributeValueDto productAttributeValueDto = null;

            if (productAttributeValue != null)
            {
                productAttributeValueDto = productAttributeValue.ToDto();
                if (productAttributeValue.ImageSquaresPictureId > 0)
                {
                    var imageSquaresPicture =
                        _pictureService.GetPictureById(productAttributeValue.ImageSquaresPictureId);
                    var imageDto = PrepareImageDto(imageSquaresPicture);
                    productAttributeValueDto.ImageSquaresImage = imageDto;
                }

                if (productAttributeValue.PictureId > 0)
                {
                    // make sure that the picture is mapped to the product
                    // This is needed since if you delete the product picture mapping from the nopCommerce administrationthe
                    // then the attribute value is not updated and it will point to a picture that has been deleted
                    var productPicture =
                      _productPictureService.GetProductPicturesByProductId(product.Id).FirstOrDefault(pp => pp.PictureId == productAttributeValue.PictureId);
                    if (productPicture != null)
                    {
                        productAttributeValueDto.ProductPictureId = productPicture.Id;
                    }
                }
            }

            return productAttributeValueDto;
        }

        private void PrepareProductAttributeCombinations(
            IEnumerable<ProductAttributeCombination> productAttributeCombinations,
            ProductDto productDto)
        {
            productDto.ProductAttributeCombinations = productDto.ProductAttributeCombinations ?? new List<ProductAttributeCombinationDto>();

            foreach (var productAttributeCombination in productAttributeCombinations)
            {
                var productAttributeCombinationDto = PrepareProductAttributeCombinationDto(productAttributeCombination);
                if (productAttributeCombinationDto != null)
                {
                    productDto.ProductAttributeCombinations.Add(productAttributeCombinationDto);
                }
            }
        }

        private ProductAttributeCombinationDto PrepareProductAttributeCombinationDto(ProductAttributeCombination productAttributeCombination)
        {
            return productAttributeCombination.ToDto();
        }

        public void PrepareProductSpecificationAttributes(IEnumerable<ProductSpecificationAttribute> productSpecificationAttributes, ProductDto productDto)
        {
            if (productDto.ProductSpecificationAttributes == null)
            {
                productDto.ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();
            }

            foreach (var productSpecificationAttribute in productSpecificationAttributes)
            {
                var productSpecificationAttributeDto = PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

                if (productSpecificationAttributeDto != null)
                {
                    productDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);
                }
            }
        }
    }
}
