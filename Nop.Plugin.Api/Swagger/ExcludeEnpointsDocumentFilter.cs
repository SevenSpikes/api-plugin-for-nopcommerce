using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Nop.Plugin.Api.Swagger
{
    public class ExcludeEnpointsDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Remove("/OAuth/Authorize");
        }
    }
}