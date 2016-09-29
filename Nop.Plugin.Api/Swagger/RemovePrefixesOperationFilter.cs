using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Nop.Plugin.Api.Swagger
{
    // This operation filter is applyed when endpoints are generated.
    public class RemovePrefixesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // This will remove all prefixes for the request parameters. 
            // i.e. if we pass limit parameter, with our current implementaion it will be passed as parameters.limit
            // we need to remove the 'parameters.' prefix otherwise the model binder won't be able to bind the parameter.
            if (operation.parameters != null)
            {
                foreach (var parameter in operation.parameters)
                {
                    string input = parameter.name;
                    int index = input.IndexOf('.');

                    if (index >= 0)
                    {
                        parameter.name = input.Substring(index + 1);
                    }
                }
            }
        }
    }
}