using System;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Nop.Plugin.Api.Swagger
{
    // This operation filter is responsible for type changing for any parameters that require such thing.
    public class ChangeParameterTypeOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // In our current implementation we requere type change only for the publishedStatus
            // because in the model binder we have special logic for it, that requires it to be 'published' or 'unpublished'
            // and because in the model it is boolean, swagger generates only boolean options in the public area and we are able to send
            // only true or false, which does not correspond with our logic in the binder, that is why we need to change the type to string
            // so the user will be able to enter the valid status values.
            if (operation.parameters != null)
            {
                foreach (var parameter in operation.parameters)
                {
                    if (parameter.name.Equals("published_status", StringComparison.InvariantCultureIgnoreCase))
                    {
                        parameter.type = "string";
                    }

                    if(parameter.name.Equals("status", StringComparison.InvariantCultureIgnoreCase) ||
                       parameter.name.Equals("payment_status", StringComparison.InvariantCultureIgnoreCase) ||
                       parameter.name.Equals("shipping_status", StringComparison.InvariantCultureIgnoreCase))
                    {
                        parameter.type = "string";
                        parameter.@enum = null;
                    }

                    if (parameter.name.Equals("ids", StringComparison.InvariantCultureIgnoreCase))
                    {
                        parameter.type = "string";
                    }
                }
            }
        }
    }
}