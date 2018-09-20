using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class OrderDtoValidator : BaseDtoValidator<OrderDto>
    {

        #region Constructors

        public OrderDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
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