
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.DTOs.ProductImages
{
    public class ProductPicturesRootObjectDto : ISerializableObject
    {
        public ProductPicturesRootObjectDto()
        {
            //Image = new ProductPicture();
        }

        [JsonProperty("image")]
        public ImageMappingDto Image { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "image";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ImageMappingDto);
        }
    }
}