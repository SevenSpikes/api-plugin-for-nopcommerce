using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

namespace Nop.Plugin.Api.Validators
{
    public abstract class BaseDtoValidator<T> : AbstractValidator<T> where T : BaseDto, new()
    {

        #region Private Fields

        private Dictionary<string, object> _requestValuesDictionary;

        #endregion

        #region Constructors

        public BaseDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestValuesDictionary)
        {
            HttpContextAccessor = httpContextAccessor;
            JsonHelper = jsonHelper;

            _requestValuesDictionary = requestValuesDictionary.Count > 0 ? requestValuesDictionary : null; //this is a hack because we can't make requestValuesDictionary an optinoal parameter, because Nop will try to resolve it)

            HttpMethod = new HttpMethod(HttpContextAccessor.HttpContext.Request.Method);

            SetRequiredIdRule();
        }

        #endregion

        #region Protected Properties

        protected IHttpContextAccessor HttpContextAccessor { get; private set; }

        protected HttpMethod HttpMethod { get; private set; }

        protected Dictionary<string, object> RequestJsonDictionary
        {
            get
            {
                if (_requestValuesDictionary == null)
                {
                    _requestValuesDictionary = GetRequestValuesDictionary();
                }

                return _requestValuesDictionary;
            }
        }

        protected IJsonHelper JsonHelper { get; private set; }

        #endregion

        #region Protected Methods

        protected void MergeValidationResult(CustomContext validationContext, ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
                {
                    validationContext.AddFailure(validationFailure);
                }
            }
        }

        protected Dictionary<string, object> GetRequestJsonDictionaryCollectionItemDictionary<TDto>(string collectionKey, TDto dto) where TDto : BaseDto
        {
            var collectionItems = (List<object>)RequestJsonDictionary[collectionKey];
            var collectionItemDictionary = (Dictionary<string, object>)collectionItems.FirstOrDefault(x => ((int)(long)((Dictionary<string, object>)x)["id"]) == dto.Id);

            return collectionItemDictionary;
        }

        protected void SetGreaterThanZeroCreateOrUpdateRule(Expression<Func<T, int?>> expression, string errorMessage, string requestValueKey)
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey(requestValueKey))
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

        protected void SetNotNullOrEmptyCreateOrUpdateRule(Expression<Func<T, string>> expression, string errorMessage, string requestValueKey)
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey(requestValueKey))
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

        private Dictionary<string, object> GetRequestValuesDictionary()
        {
            var requestJsonDictionary = JsonHelper.GetRequestJsonDictionaryFromStream(HttpContextAccessor.HttpContext.Request.Body, true);
            var rootPropertyName = JsonHelper.GetRootPropertyName<T>();

            if (requestJsonDictionary.ContainsKey(rootPropertyName))
            {
                requestJsonDictionary = (Dictionary<string, object>)requestJsonDictionary[rootPropertyName];
            }

            return requestJsonDictionary;
        }

        private void SetRequiredIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(x => x.Id, "invalid id", "id");
        }

        #endregion

    }
}