using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrders
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrders_StatusesParameters
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
        public void WhenCalledWithSomeOrderStatus_GivenOrdersCollectionContainingOrdersHavingThisStatus_ShouldReturnTheOrdersHavingThisStatusOrderedById(OrderStatus orderStatus)
        {
            // Arange
            var expectedCollection = _existigOrders.Where(x => x.OrderStatus == orderStatus).OrderBy(x => x.Id);

            // Act
            var orders = _orderApiService.GetOrders(status: orderStatus);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(PaymentStatus.Paid)]
        [TestCase(PaymentStatus.Refunded)]
        public void WhenCalledWithSomePaymentStatus_GivenOrdersCollectionContainingOrdersHavingThisStatus_ShouldReturnTheOrdersHavingThisStatusOrderedById(PaymentStatus paymentStatus)
        {
            // Arange
            var expectedCollection = _existigOrders.Where(x => x.PaymentStatus == paymentStatus).OrderBy(x => x.Id);

            // Act
            var orders = _orderApiService.GetOrders(paymentStatus: paymentStatus);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(ShippingStatus.Delivered)]
        [TestCase(ShippingStatus.ShippingNotRequired)]
        public void WhenCalledWithSomeShippingStatus_GivenOrdersCollectionContainingOrdersHavingThisStatus_ShouldReturnTheOrdersHavingThisStatusOrderedById(ShippingStatus shippingStatus)
        {
            // Arange
            var expectedCollection = _existigOrders.Where(x => x.ShippingStatus == shippingStatus).OrderBy(x => x.Id);

            // Act
            var orders = _orderApiService.GetOrders(shippingStatus: shippingStatus);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithStatus_GivenOrdersCollectionThatDoNotHaveThisStatus_ShouldReturnEmptyCollection()
        {
            // Arange
            var orderStatus = OrderStatus.Processing;
            var paymentStatus = PaymentStatus.PartiallyRefunded;
            var shippingStatus = ShippingStatus.Shipped;

            // Act
            var orders = new List<Order>();

            List<Order> ordersByOrderStatus = _orderApiService.GetOrders(status: orderStatus).ToList();
            List<Order> ordersByPaymentStatus = _orderApiService.GetOrders(paymentStatus: paymentStatus).ToList();
            List<Order> ordersByShippingStatus = _orderApiService.GetOrders(shippingStatus: shippingStatus).ToList();

            orders.AddRange(ordersByOrderStatus);
            orders.AddRange(ordersByPaymentStatus);
            orders.AddRange(ordersByShippingStatus);

            // Assert
            CollectionAssert.IsEmpty(orders);
        }
    }
}