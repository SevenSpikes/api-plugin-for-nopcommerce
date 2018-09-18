using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class OrderDtoValidator : BaseDtoValidator<OrderDto>
    {

        #region Constructors

        public OrderDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper) : base(httpContextAccessor, jsonHelper)
        {
            SetCustomerIdRule();
        }

        #endregion

        #region Private Methods

        private void SetCustomerIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.CustomerId, "invalid customer_id", "customer_id");
        }

        #endregion

    }
}