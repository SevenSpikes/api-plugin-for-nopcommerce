﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.SpecificationAttributesParameters
{
    [ModelBinder(typeof(ParametersModelBinder<SpecificationAttributesParametersModel>))]
    public class SpecificationAttributesParametersModel
    {
        public SpecificationAttributesParametersModel()
        {
            Limit = Constants.Configurations.DefaultLimit;
            Page = Constants.Configurations.DefaultPageValue;
            SinceId = Constants.Configurations.DefaultSinceId;
        }

        /// <summary>
        ///     Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        ///     Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        ///     comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }

        /// <summary>
        ///     Restrict results to after the specified ID
        /// </summary>
        [JsonProperty("since_id")]
        public int SinceId { get; set; }
    }
}
