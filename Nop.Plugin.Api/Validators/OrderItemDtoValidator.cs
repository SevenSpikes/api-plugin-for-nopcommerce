using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class OrderItemDtoValidator : BaseDtoValidator<OrderItemDto>
    {

        #region Constructors

        public OrderItemDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper) : base(httpContextAccessor, jsonHelper)
        {
            SetProductRule();
            SetQuantityRule();
        }

        #endregion

        #region Private Methods

        private void SetProductRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.ProductId, "invalid product_id", "product_id");
        }

        private void SetQuantityRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.Quantity, "invalid quanitty", "quantity");
        }

        #endregion

    }
}