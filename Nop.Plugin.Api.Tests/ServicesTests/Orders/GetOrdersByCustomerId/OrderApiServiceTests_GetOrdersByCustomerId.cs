using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrdersByCustomerId
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrdersByCustomerId
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
        [TestCase(0)]
        [TestCase(-30)]
        public void WhenZeroOrNegativeCustomerIdPassed_ShouldReturnEmptyCollection(int negativeOrZeroCustomerId)
        {
            // Act  
            var result = _orderApiService.GetOrdersByCustomerId(negativeOrZeroCustomerId);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        [TestCase(34988934)]
        [TestCase(98472)]
        public void WhenNonExistingCustomerIdPassed_ShouldReturnEmptyCollection(int nonExistingCustomerId)
        {
            // Act  
            var result = _orderApiService.GetOrdersByCustomerId(nonExistingCustomerId);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void WhenExistingCustomerIdPassed_ShouldReturnTheNonDeletedOrdersOrderedById(int existingCustomerId)
        {
            // Arange
            var expectedCollection = _existigOrders.Where(x => x.CustomerId == existingCustomerId && !x.Deleted).OrderBy(x => x.Id);

            // Act  
            var result = _orderApiService.GetOrdersByCustomerId(existingCustomerId);

            // Assert
            CollectionAssert.IsNotEmpty(result);
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }
    }
}
