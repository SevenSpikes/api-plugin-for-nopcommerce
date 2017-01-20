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

        public DTOHelper(IProductService productService, IAclService aclService, IStoreMappingService storeMappingService, IPictureService pictureService)
        {
            _productService = productService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
        }

        public ProductDto PrepareProductDTO(Product product)
        {
            ProductDto productDto = product.ToDto();

            PrepareProductImages(product.ProductPictures, productDto);

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
            ImageDto imageDto = PrepareImageDto(picture, categoryDto);

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
                productDto.Images = new List<ImageDto>();

            // Here we prepare the resulted dto image.
            foreach (var productPicture in productPictures)
            {
                ImageDto imageDto = PrepareImageDto(productPicture.Picture, productDto);

                if (imageDto != null)
                {
                    imageDto.Id = productPicture.Id;
                    imageDto.Position = productPicture.DisplayOrder;
                    productDto.Images.Add(imageDto);
                }
            }
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
