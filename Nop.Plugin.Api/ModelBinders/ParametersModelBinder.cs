using System.Collections.Generic;
using Nop.Plugin.Api.Converters;

namespace Nop.Plugin.Api.ModelBinders
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    // The idea comes from this article http://can-we-code-it.blogspot.co.uk/2015/04/handling-put-content-of-any-mime-type.html
    // but instead of using streams I am using the properties of the request.
    public class ParametersModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IObjectConverter _objectConverter;

        public ParametersModelBinder(IObjectConverter objectConverter)
        {
            _objectConverter = objectConverter;
        }
        
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // TODO: find where the properties are
            //// MS_QueryNameValuePairs contains key value pair representation of the query parameters passed to in the request.
            //if (bindingContext.HttpContext.Request.Properties.ContainsKey("MS_QueryNameValuePairs"))
            //{
            //    bindingContext.Model = _objectConverter.ToObject<T>(
            //        (ICollection<KeyValuePair<string, string>>)actionContext.Request.Properties["MS_QueryNameValuePairs"]);
            //}
            //else
            //{
            //    bindingContext.Model = new T();
            //}

            // This should be true otherwise the model will be null.
            return Task.CompletedTask;
        }
    }
}