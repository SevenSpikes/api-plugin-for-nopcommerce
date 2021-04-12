﻿using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTO.Manufacturers;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    [UsedImplicitly]
    public class ManufacturerDtoValidator : BaseDtoValidator<ManufacturerDto>
    {
        #region Constructors

        public ManufacturerDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) :
            base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(m => m.Name, "invalid name", "name");
        }

        #endregion
    }
}
