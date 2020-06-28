using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using static Nop.Plugin.Api.Infrastructure.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface IOrderApiService
    {
        IList<Order> GetOrdersByCustomerId(int customerId);

        IList<Order> GetOrders(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                               int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, 
                               int sinceId = Configurations.DefaultSinceId, OrderStatus? status = null, PaymentStatus? paymentStatus = null, 
                               ShippingStatus? shippingStatus = null, int? customerId = null, int? storeId = null);

        Order GetOrderById(int orderId);

        int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
                           PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
                           int? customerId = null, int? storeId = null, int sinceId = Configurations.DefaultSinceId);
    }
}