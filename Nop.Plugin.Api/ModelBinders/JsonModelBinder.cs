using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation.Attributes;
using FluentValidation.Results;
using Newtonsoft.Json;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.Validators;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.ModelBinders
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class JsonModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly ILocalizationService _localizationService;
        private readonly int FirstLanguageId;

        public JsonModelBinder(IJsonHelper jsonHelper, ILocalizationService localizationService, ILanguageService languageService)
        {
            _jsonHelper = jsonHelper;
            _localizationService = localizationService;

            // Languages are ordered by display order so the first language will be with the smallest display order.
            Language firstLanguage = languageService.GetAllLanguages().FirstOrDefault();

            if (firstLanguage != null)
            {
                FirstLanguageId = firstLanguage.Id;
            }
            else
            {
                FirstLanguageId = 0;
            }
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Dictionary<string, object> result = GetResult(bindingContext);

            if (result == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
            
            string rootProperty = GetRootProperty(bindingContext);

            // Now we need to validate the root property.
            ValidateRootProperty(bindingContext, result, rootProperty);

            if (bindingContext.ModelState.IsValid)
            {
                // The validation for the key is in the Validate method.
                Dictionary<string, object> propertyValuePaires =
                    (Dictionary<string, object>) result[rootProperty];

                // You will have id parameter passed in the model binder only when you have put request.
                // because get and delete do not use the model binder.
                // Here we insert the id in the property value pairs to be validated by the dto validator in a later point.
                object routeDataId = GetRouteDataId(bindingContext.ActionContext);

                if (routeDataId != null)
                {
                    // Here we insert the route data id in the value paires.
                    // If id is contained in the category json body the one from the route data is used instead.
                    InsertIdInTheValuePaires(propertyValuePaires, routeDataId);
                }

                // We need to call this method here so it will be certain that the routeDataId will be in the propertyValuePaires
                // when the request is PUT.
                ValidateValueTypes(bindingContext, propertyValuePaires);

                Delta<T> delta = null;

                if (bindingContext.ModelState.IsValid)
                {
                    delta = new Delta<T>(propertyValuePaires);
                    ValidateModel(bindingContext, propertyValuePaires, delta.Dto);
                }

                if (bindingContext.ModelState.IsValid)
                {
                    bindingContext.Model = delta;
                    bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }

        private Dictionary<string, object> GetResult(ModelBindingContext bindingContext)
        {
            Dictionary<string, object> result = null;

            Stream requestPayloadStream = bindingContext.ActionContext.HttpContext.Request.Body;

            string requestPayload = string.Empty;

            using (requestPayloadStream)
            {
                if (requestPayloadStream != null)
                {
                    var streamReader = new StreamReader(requestPayloadStream);
                    requestPayload = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }

            // We need to check if the request has a payload.
            CheckIfJsonIsProvided(bindingContext, requestPayload);

            // After we are sure that the request payload and json are provided we need to deserialize this json.
            result = DeserializeReqestPayload(bindingContext, requestPayload);

            // Next we have to validate the json format.
            ValidateJsonFormat(bindingContext, result);

            return result;
        }

        private object GetRouteDataId(ActionContext actionContext)
        {
            object routeDataId = null;

            if (actionContext.RouteData.Values.ContainsKey("id"))
            {
                routeDataId = actionContext.RouteData.Values["id"];
            }

            return routeDataId;
        }

        private void ValidateValueTypes(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePaires)
        {
            var errors = new Dictionary<string, string>();

            // Validate if the property value pairs passed maches the type.
            var typeValidator = new TypeValidator<T>();

            if (!typeValidator.IsValid(propertyValuePaires))
            {
                foreach (var invalidProperty in typeValidator.InvalidProperties)
                {
                    var key = string.Format(_localizationService.GetResource("Api.InvalidType", FirstLanguageId, false), invalidProperty);

                    if (!errors.ContainsKey(key))
                    {
                        errors.Add(key, _localizationService.GetResource("Api.InvalidPropertyType", FirstLanguageId, false));
                    }
                }
            }
            
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    bindingContext.ModelState.AddModelError(error.Key, error.Value);
                }
            }
        }

        private void ValidateRootProperty(ModelBindingContext bindingContext, Dictionary<string, object> result, string rootProperty)
        {
            if (bindingContext.ModelState.IsValid)
            {
                bool isRootPropertyValid = !string.IsNullOrEmpty(rootProperty) && result.ContainsKey(rootProperty);

                if (!isRootPropertyValid)
                {
                    bindingContext.ModelState.AddModelError("rootProperty", _localizationService.GetResource("Api.InvalidRootProperty", FirstLanguageId, false));
                }
            }
        }

        private string GetRootProperty(ModelBindingContext bindingContext)
        {
            string rootProperty = null;

            if (bindingContext.ModelState.IsValid)
            {
                JsonObjectAttribute jsonObjectAttribute = ReflectionHelper.GetJsonObjectAttribute(typeof(T));

                if (jsonObjectAttribute != null)
                {
                    rootProperty = jsonObjectAttribute.Title;
                }
            }

            return rootProperty;
        }

        private void ValidateJsonFormat(ModelBindingContext bindingContext, Dictionary<string, object> result)
        {
            bool isJsonFormatValid = result != null && result.Count > 0;

            if (!isJsonFormatValid)
            {
                bindingContext.ModelState.AddModelError("json",
                    _localizationService.GetResource("Api.InvalidJsonFormat", FirstLanguageId, false));
            }
        }

        private Dictionary<string, object> DeserializeReqestPayload(ModelBindingContext bindingContext, string requestPayload)
        {
            Dictionary<string, object> result = null;

            // Here we check if validation has passed to this point.
            if (bindingContext.ModelState.IsValid)
            {
                result = _jsonHelper.DeserializeToDictionary(requestPayload);
            }

            return result;
        }

        private void CheckIfJsonIsProvided(ModelBindingContext bindingContext, string requestPayload)
        {
            if (string.IsNullOrEmpty(requestPayload) &&
                bindingContext.ModelState.IsValid)
            {
                bindingContext.ModelState.AddModelError("json", _localizationService.GetResource("Api.NoJsonProvided", FirstLanguageId, false));
            }
        }
        
        private void ValidateModel(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePaires, T dto)
        {
            ValidationResult validationResult = GetValidationResult(bindingContext.ActionContext, propertyValuePaires, dto);

            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
                {
                    bindingContext.ModelState.AddModelError(validationFailure.PropertyName,
                        validationFailure.ErrorMessage);
                }
            }
            else
            {
                HandleValidationAttributes(dto, bindingContext);
            }
        }

        private ValidationResult GetValidationResult(ActionContext actionContext, Dictionary<string, object> propertyValuePaires, T dto)
        {
            var validationResult = new ValidationResult();

            // Needed so we can call the get the validator.
            ValidatorAttribute validatorAttribute =
                typeof (T).GetCustomAttribute(typeof (ValidatorAttribute)) as ValidatorAttribute;

            if (validatorAttribute != null)
            {
                Type validatorType = validatorAttribute.ValidatorType;

                // We need to pass the http method because there are some differences between the validation rules for post and put
                // We need to pass the propertyValuePaires from the passed json because there are cases in which one field is required
                // on post, but it is a valid case not to pass it when doing a put request.    
                var validator = Activator.CreateInstance(validatorType,
                    new object[]
                    {
                        //TODO: find this
                        actionContext.HttpContext.Request.Method,
                        propertyValuePaires
                    });

                // We know that the validator will be AbstractValidator<T> which means it will have Validate method.
                validationResult = validatorType.GetMethod("Validate", new[] {typeof (T)})
                    .Invoke(validator, new[] {dto}) as ValidationResult;
            }

            return validationResult;
        }

        private void HandleValidationAttributes(T dto, ModelBindingContext bindingContext)
        {
            var dtoProperties = dto.GetType().GetProperties();

            foreach (var property in dtoProperties)
            {
                // Check property type
                BaseValidationAttribute validationAttribute = property.PropertyType.GetCustomAttribute(typeof (BaseValidationAttribute)) as BaseValidationAttribute;

                // If not on property type, check the property itself.
                if (validationAttribute == null)
                {
                    validationAttribute = property.GetCustomAttribute(typeof (BaseValidationAttribute)) as BaseValidationAttribute;
                }

                if (validationAttribute != null)
                {
                    validationAttribute.Validate(property.GetValue(dto));
                    Dictionary<string, string> errors = validationAttribute.GetErrors();

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            bindingContext.ModelState.AddModelError(error.Key, error.Value);
                        }
                    }
                }
            }
        }

        private void InsertIdInTheValuePaires(Dictionary<string, object> propertyValuePaires, object requestId)
        {
            if (propertyValuePaires.ContainsKey("id"))
            {
                propertyValuePaires["id"] = requestId;
            }
            else
            {
                propertyValuePaires.Add("id", requestId);
            }
        }
    }
}