// -----------------------------------------------------------------------
// <copyright from="2018" to="2018" file="NopApiClient.cs" company="Lindell Technologies">
//    Copyright (c) Lindell Technologies All Rights Reserved.
//    Information Contained Herein is Proprietary and Confidential.
// </copyright>
// -----------------------------------------------------------------------


using Nop.Plugin.Api.Client.Clients;

namespace Nop.Plugin.Api.Client
{
    public class NopApiClient
    {
        private OrdersClient _ordersClient;

        public NopApiClient(string serverUrl, string clientId, string secret)
        {
            ServerUrl = serverUrl;
            ClientId = clientId;
            Secret = secret;
        }
        
        public string ServerUrl { get; }

        public string ClientId { get; }
        
        public string Secret { get; }

        public OrdersClient Orders => _ordersClient ?? (_ordersClient = new OrdersClient(this));
    }
}