// -----------------------------------------------------------------------
// <copyright from="2018" to="2018" file="OrdersClient.cs" company="Lindell Technologies">
//    Copyright (c) Lindell Technologies All Rights Reserved.
//    Information Contained Herein is Proprietary and Confidential.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Plugin.Api.Client.DTOs.Orders;

namespace Nop.Plugin.Api.Client.Clients
{
    public class OrdersClient : BaseRestClient
    {
        internal OrdersClient(NopApiClient parent) : base(parent)
        {
        }

        public IList<OrderDto> GetOrders(int sinceId = 0, DateTime? createdAtMin = null, DateTime? createdAtMax = null, int? customerId = null, int page = 1, int limit = 50)
        {
            var request = GetRequest("api/orders");

            
            if (sinceId > 0)
            {
                request.AddQueryParameter("sinceId", sinceId.ToString());
            }

            if (createdAtMin.HasValue)
            {
                request.AddQueryParameter("createdAtMin", createdAtMin.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (createdAtMax.HasValue)
            {
                request.AddQueryParameter("createdAtMax", createdAtMax.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (customerId.GetValueOrDefault() > 0)
            {
                request.AddQueryParameter("customerId", customerId.GetValueOrDefault().ToString());
            }

            if (page > 0)
            {
                request.AddQueryParameter("page", page.ToString());
            }

            if (limit > 0)
            {
                request.AddQueryParameter("limit", Math.Min(250, limit).ToString());
            }


            return Execute<OrdersRootObject>(request).Data.Orders;
        }

        public IList<OrderDto> GetOrders(IEnumerable<int> ids)
        {
            var request = GetRequest("api/orders");

            if (ids != null)
            {
                request.AddQueryParameter("ids", string.Join(",", ids.Select(s => s.ToString())));
            }

            return Execute<OrdersRootObject>(request).Data.Orders;
        }
    }
}