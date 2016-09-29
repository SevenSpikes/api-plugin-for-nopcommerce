using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.Converters;

namespace Nop.Plugin.Api.ModelBinders
{
    // The idea comes from this article http://can-we-code-it.blogspot.co.uk/2015/04/handling-put-content-of-any-mime-type.html
    // but instead of using streams I am using the properties of the request.
    public class ParametersModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IObjectConverter _objectConverter;

        public ParametersModelBinder(IObjectConverter objectConverter)
        {
            _objectConverter = objectConverter;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            // MS_QueryNameValuePairs contains key value pair representation of the query parameters passed to in the request.
            if (actionContext.Request.Properties.ContainsKey("MS_QueryNameValuePairs"))
            {
                bindingContext.Model = _objectConverter.ToObject<T>(
                    (ICollection<KeyValuePair<string, string>>)actionContext.Request.Properties["MS_QueryNameValuePairs"]);
            }
            else
            {
                bindingContext.Model = new T();
            }

            // This should be true otherwise the model will be null.
            return true;
        }
    }
}