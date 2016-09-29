using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrdersCount
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrdersCount_StatusesParameters
    {
        private IOrderApiService _orderApiService;
        private List<Order> _existigOrders;

        [SetUp]
        public void Setup()
        {
            _existigOrders = new List<Order>()
            {
                new Order() {Id = 2, OrderStatus = OrderStatus.Complete },
                new Order() {Id = 3, PaymentStatus = PaymentStatus.Paid },
                new Order() {Id = 1, ShippingStatus = ShippingStatus.Delivered },
                new Order() {Id = 4, OrderStatus = OrderStatus.Cancelled, PaymentStatus = PaymentStatus.Pending },
                new Order() {Id = 5, ShippingStatus = ShippingStatus.NotYetShipped, PaymentStatus = PaymentStatus.Paid },
                new Order() {Id = 6, OrderStatus = OrderStatus.Complete, ShippingStatus = ShippingStatus.ShippingNotRequired },
                new Order() {Id = 7, OrderStatus = OrderStatus.Cancelled, PaymentStatus = PaymentStatus.Refunded, ShippingStatus = ShippingStatus.NotYetShipped }
            };

            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(_existigOrders.AsQueryable());

            _orderApiService = new OrderApiService(orderRepo);
        }

        [Test]
        [TestCase(OrderStatus.Complete)]
        [TestCase(OrderStatus.Cancelled)]
        public void WhenCalledWithSomeOrderStatus_GivenOrdersCollectionContainingOrdersHavingThisStatus_ShouldReturnTheCountOfTheOrdersHavingThisStatus(OrderStatus orderStatus)
        {
            // Arange
            var expectedCollectionCount = _existigOrders.Count(x => x.OrderStatus == orderStatus);

            // Act
            var resultCount = _orderApiService.GetOrdersCount(status: orderStatus);

            // Assert
            Assert.AreEqual(expectedCollectionCount, resultCount);
        }

        [Test]
        [TestCase(PaymentStatus.Paid)]
        [TestCase(PaymentStatus.Refunded)]
        public void WhenCalledWithSomePaymentStatus_GivenOrdersCollectionContainingOrdersHavingThisStatus_ShouldReturnTheCountOfTheOrdersHavingThisStatus(PaymentStatus paymentStatus)
        {
            // Arange
            var expectedCollectionCount = _existigOrders.Count(x => x.PaymentStatus == paymentStatus);

            // Act
            var resultCount = _orderApiService.GetOrdersCount(paymentStatus: paymentStatus);

            // Assert
            Assert.AreEqual(expectedCollectionCount, resultCount);
        }

        [Test]
        [TestCase(ShippingStatus.Delivered)]
        [TestCase(ShippingStatus.ShippingNotRequired)]
        public void WhenCalledWithSomeShippingStatus_GivenOrdersCollectionContainingOrdersHavingThisStatus_ShouldReturnTheCountOfTheOrdersHavingThisStatus(ShippingStatus shippingStatus)
        {
            // Arange
            var expectedCollectionCount = _existigOrders.Count(x => x.ShippingStatus == shippingStatus);

            // Act
            var resultCount = _orderApiService.GetOrdersCount(shippingStatus: shippingStatus);

            // Assert
            Assert.AreEqual(expectedCollectionCount, resultCount);
        }

        [Test]
        public void WhenCalledWithStatus_GivenOrdersCollectionThatDoNotHaveThisStatus_ShouldReturnZero()
        {
            // Arange
            var orderStatus = OrderStatus.Processing;
            var paymentStatus = PaymentStatus.PartiallyRefunded;
            var shippingStatus = ShippingStatus.Shipped;

            // Act
            var resultCount = _orderApiService.GetOrdersCount(status: orderStatus) + 
                              _orderApiService.GetOrdersCount(paymentStatus: paymentStatus) +
                              _orderApiService.GetOrdersCount(shippingStatus: shippingStatus); 

            // Assert
            Assert.AreEqual(0, resultCount);
        }
    }
}