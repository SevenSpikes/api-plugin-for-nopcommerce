using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;

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

            HttpMethod = new HttpMethod(_httpContextAccessor.HttpContext.Request.Method);

            SetRequiredIdRule();
        }

        #endregion

        #region Protected Properties

        protected HttpMethod HttpMethod { get; private set; }

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

        protected void SetGreaterThanZeroCreateOrUpdateRule(Expression<Func<T, int?>> expression, string errorMessage, string jsonKey)
        {
            if (HttpMethod == HttpMethod.Post || JsonDictionary.ContainsKey(jsonKey))
            {
                SetGreaterThanZeroRule(expression, errorMessage);
            }
        }

        protected void SetGreaterThanZeroRule(Expression<Func<T, int?>> expression, string errorMessage)
        {
            RuleFor(expression)
                .NotNull()
                .NotEmpty()
                .Must(id => id > 0);
        }

        protected void SetNotNullOrEmptyCreateOrUpdateRule(Expression<Func<T, string>> expression, string errorMessage, string jsonKey)
        {
            if (HttpMethod == HttpMethod.Post || JsonDictionary.ContainsKey(jsonKey))
            {
                SetNotNullOrEmptyRule(expression, errorMessage);
            }
        }

        protected void SetNotNullOrEmptyRule(Expression<Func<T, string>> expression, string errorMessage)
        {
            RuleFor(expression)
                .NotNull()
                .NotEmpty()
                .WithMessage(errorMessage);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, object> GetJsonDictionary()
        {
            var jsonDictionary = _jsonHelper.GetJsonDictionaryFromStream(_httpContextAccessor.HttpContext.Request.Body, true);
            var rootPropertyName = _jsonHelper.GetRootPropertyName<T>();

            if (jsonDictionary.ContainsKey(rootPropertyName))
            {
                jsonDictionary = (Dictionary<string, object>)jsonDictionary[rootPropertyName];
            }

            return jsonDictionary;
        }

        private void SetRequiredIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(x => x.Id, "invalid id", "id");
        }

        #endregion

    }
}