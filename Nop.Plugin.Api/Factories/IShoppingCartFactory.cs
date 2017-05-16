using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.Factories
{
    public interface IShoppingCartFactory<out T>
    {
        T CreateFor(ShoppingCartItemDto model);

    }
}
