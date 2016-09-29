using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrdersCount
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrdersCount_CustomerIdParameter
    {
        private IOrderApiService _orderApiService;
        private List<Order> _existigOrders;

        [SetUp]
        public void Setup()
        {
            _existigOrders = new List<Order>()
            {
                new Order() {Id = 2, CustomerId = 1},
                new Order() {Id = 3, CustomerId = 2},
                new Order() {Id = 1, CustomerId = 2},
                new Order() {Id = 4, CustomerId = 1},
                new Order() {Id = 5, CustomerId = 1},
                new Order() {Id = 6, CustomerId = 2},
                new Order() {Id = 7, CustomerId = 1, Deleted = true}
            };

            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(_existigOrders.AsQueryable());

            _orderApiService = new OrderApiService(orderRepo);
        }

        [Test]
        public void WhenCalledWithValidCustomerId_ShouldReturnTheCountOfTheOrdersForThisCustomer()
        {
            // Arange
            int customerId = 1;
            var expectedCollectionCount = _existigOrders.Count(x => x.CustomerId == customerId && !x.Deleted);

            // Act
            var productsCount = _orderApiService.GetOrdersCount(customerId: customerId);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledWithNegativeOrZeroCustomerId_ShouldReturnZero(int customerId)
        {
            // Act
            var ordersCount = _orderApiService.GetOrdersCount(customerId: customerId);

            // Assert
            Assert.AreEqual(0, ordersCount);
        }

        [Test]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(5465464)]
        public void WhenCalledWithCustomerIdThatDoesNotExistInTheMappings_ShouldReturnZero(int customerId)
        {
            // Act
            var ordersCount = _orderApiService.GetOrdersCount(customerId: customerId);

            // Assert
            Assert.AreEqual(0, ordersCount);
        }
    }
}