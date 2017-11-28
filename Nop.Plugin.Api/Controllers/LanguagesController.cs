﻿using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    using System.Net;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class LanguagesController : BaseApiController
    {
        private ILanguageService _languageService;
        private readonly IDTOHelper _dtoHelper;

        public LanguagesController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ILanguageService languageService,
            IDTOHelper dtoHelper)
            : base(jsonFieldsSerializer,
                  aclService,
                  customerService,
                  storeMappingService,
                  storeService,
                  discountService,
                  customerActivityService,
                  localizationService,
                  pictureService)
        {
            _languageService = languageService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Retrieve all languages
        /// </summary>
        /// <param name="fields">Fields from the language you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/languages")]
        [ProducesResponseType(typeof(LanguagesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllLanguages(string fields = "")
        {
            IList<Language> allLanguages = _languageService.GetAllLanguages();

            IList<LanguageDto> languagesAsDto = allLanguages.Select(language => _dtoHelper.PrepateLanguageDto(language)).ToList();

            var languagesRootObject = new LanguagesRootObject()
            {
                Languages = languagesAsDto
            };

            var json = _jsonFieldsSerializer.Serialize(languagesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
