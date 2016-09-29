using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrders
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrders_CustomerIdParameter
    {
        private IOrderApiService _orderApiService;
        private List<Order> _existigOrders;
        
        [SetUp]
        public void Setup()
        {
            _existigOrders = new List<Order>()
            {
                new Order() {Id = 2, CustomerId = 1},
                new Order() {Id = 3, CustomerId = 1},
                new Order() {Id = 1, CustomerId = 2},
                new Order() {Id = 4, CustomerId = 1},
                new Order() {Id = 5, CustomerId = 2},
                new Order() {Id = 6, CustomerId = 1, Deleted = true},
                new Order() {Id = 7, CustomerId = 2}
            };

            var productRepo = MockRepository.GenerateStub<IRepository<Order>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigOrders.AsQueryable());
            
            _orderApiService = new OrderApiService(productRepo);
        }

        [Test]
        public void WhenCalledWithValidCustomerId_ShouldReturnOnlyTheOrdersForThisCustomer()
        {
            // Arange
            int customerId = 1;
            var expectedCollection =
                _existigOrders.Where(x => x.CustomerId == customerId && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var orders = _orderApiService.GetOrders(customerId: customerId);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledWithNegativeOrZeroCustomerId_ShouldReturnEmptyCollection(int customerId)
        {
            // Act
            var orders = _orderApiService.GetOrders(customerId: customerId);

            // Assert
            CollectionAssert.IsEmpty(orders);
        }

        [Test]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(5465464)]
        public void WhenCalledWithCustomerIdThatDoesNotExistInTheMappings_ShouldReturnEmptyCollection(int customerId)
        {
            // Act
            var orders = _orderApiService.GetOrders(customerId: customerId);

            // Assert
            CollectionAssert.IsEmpty(orders);
        }
    }
}