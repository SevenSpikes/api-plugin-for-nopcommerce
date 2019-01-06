using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.OrdersParameters
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Nop.Plugin.Api.Constants;

    [ModelBinder(typeof(ParametersModelBinder<OrdersCountParametersModel>))]
    public class OrdersCountParametersModel : BaseOrdersParametersModel
    {
        public OrdersCountParametersModel()
        {
            SinceId = Configurations.DefaultSinceId;
        }

        /// <summary>
        /// Restrict results to after the specified ID
        /// </summary>
        [JsonProperty("since_id")]
        public int SinceId { get; set; }
    }
}