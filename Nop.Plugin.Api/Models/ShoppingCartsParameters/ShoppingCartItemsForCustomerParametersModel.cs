using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ShoppingCartsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsForCustomerParametersModel>))]
    public class ShoppingCartItemsForCustomerParametersModel : BaseShoppingCartItemsParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}