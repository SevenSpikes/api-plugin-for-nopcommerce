using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Services.Media;

namespace Nop.Plugin.Api.Helpers
{
    public class DTOHelper : IDTOHelper
    {
        private IProductService _productService;
        private IAclService _aclService;
        private IStoreMappingService _storeMappingService;
        private IPictureService _pictureService;
        private IProductAttributeService _productAttributeService;

        public DTOHelper(IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService)
        {
            _productService = productService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
        }

        public ProductDto PrepareProductDTO(Product product)
        {
            ProductDto productDto = product.ToDto();

            PrepareProductImages(product.ProductPictures, productDto);
            PrepareProductAttributes(product.ProductAttributeMappings,productDto);

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

            return productDto;
        }

        public CategoryDto PrepareCategoryDTO(Category category)
        {
            CategoryDto categoryDto = category.ToDto();

            Picture picture = _pictureService.GetPictureById(category.PictureId);
            ImageDto imageDto = PrepareImageDto(picture);

            if (imageDto != null)
            {
                categoryDto.Image = imageDto;
            }

            categoryDto.SeName = category.GetSeName();
            categoryDto.DiscountIds = category.AppliedDiscounts.Select(discount => discount.Id).ToList();
            categoryDto.RoleIds = _aclService.GetAclRecords(category).Select(acl => acl.CustomerRoleId).ToList();
            categoryDto.StoreIds = _storeMappingService.GetStoreMappings(category).Select(mapping => mapping.StoreId).ToList();

            return categoryDto;
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

        protected ImageDto PrepareImageDto(Picture picture)
        {
            ImageDto image = null;

            if (picture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                image = new ImageDto()
                {
                    //Attachment = Convert.ToBase64String(picture.PictureBinary),
                    Src = _pictureService.GetPictureUrl(picture)
                };
            }

            return image;
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
                    ProductAttributeValues = productAttributeMapping.ProductAttributeValues.Select(x => PrepareProductAttributeValueDto(x, productAttributeMapping.Product)).ToList()
                };
            }

            return productAttributeMappingDto;
        }

        private ProductAttributeValueDto PrepareProductAttributeValueDto(ProductAttributeValue productAttributeValue, Product product)
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

    }
}
