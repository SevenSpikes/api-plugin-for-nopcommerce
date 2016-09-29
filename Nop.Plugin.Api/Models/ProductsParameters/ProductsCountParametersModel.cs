using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ProductsCountParametersModel>))]
    public class ProductsCountParametersModel : BaseProductsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}