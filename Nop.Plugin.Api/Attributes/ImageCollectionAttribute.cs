﻿using System.Collections.Generic;
using Nop.Plugin.Api.DTO.Images;

namespace Nop.Plugin.Api.Attributes
{
    public class ImageCollectionValidationAttribute : BaseValidationAttribute
    {
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public override void Validate(object instance)
        {
            // Images are not required so they could be null
            // and there is nothing to validate in this case

            if (instance is ICollection<ImageMappingDto> imagesCollection)
            {
                foreach (var image in imagesCollection)
                {
                    var imageValidationAttribute = new ImageValidationAttribute();

                    imageValidationAttribute.Validate(image);

                    var errorsForImage = imageValidationAttribute.GetErrors();

                    if (errorsForImage.Count > 0)
                    {
                        _errors = errorsForImage;
                        break;
                    }
                }
            }
        }

        public override Dictionary<string, string> GetErrors()
        {
            return _errors;
        }
    }
}
