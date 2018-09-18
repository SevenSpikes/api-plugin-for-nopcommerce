using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public abstract class BaseDtoValidator<T> : AbstractValidator<T> where T : BaseDto, new()
    {

        #region Private Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJsonHelper _jsonHelper;

        private Dictionary<string, object> _jsonDictionary;

        #endregion

        #region Constructors

        public BaseDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _jsonHelper = jsonHelper;
        }

        #endregion

        #region Protected Properties

        protected Dictionary<string, object> JsonDictionary
        {
            get
            {
                if (_jsonDictionary == null)
                {
                    _jsonDictionary = GetJsonDictionary();
                }

                return _jsonDictionary;
            }
        }

        #endregion

        #region Protected Methods

        protected void SetRequiredIdRule()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty()
                .Must(id => id > 0)
                .WithMessage("invalid id");
        }

        #endregion

        #region Private Methods

        private Dictionary<string, object> GetJsonDictionary()
        {
            var jsonDictionary = _jsonHelper.GetJsonDictionaryFromStream(_httpContextAccessor.HttpContext.Request.Body, true);
            var rootPropertyName = _jsonHelper.GetRootPropertyName<T>();
            jsonDictionary = (Dictionary<string, object>)jsonDictionary[rootPropertyName];

            return jsonDictionary;
        }

        #endregion

    }
}