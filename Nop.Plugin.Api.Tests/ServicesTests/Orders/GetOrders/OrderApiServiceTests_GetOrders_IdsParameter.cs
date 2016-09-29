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
    public class OrderApiServiceTests_GetOrders_IdsParameter
    {
        private IOrderApiService _orderApiService;
        private List<Order> _existigOrders;

        [SetUp]
        public void Setup()
        {
            _existigOrders = new List<Order>()
            {
                new Order() {Id = 2},
                new Order() {Id = 3},
                new Order() {Id = 1},
                new Order() {Id = 4},
                new Order() {Id = 5},
                new Order() {Id = 6, Deleted = true},
                new Order() {Id = 7}
            };

            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(_existigOrders.AsQueryable());
            
            _orderApiService = new OrderApiService(orderRepo);
        }
        
        [Test]
        public void WhenCalledWithIdsParameter_GivenOrdersWithTheSpecifiedIds_ShouldReturnThemSortedById()
        {
            var idsCollection = new List<int>() { 1, 5 };

            var orders = _orderApiService.GetOrders(ids: idsCollection);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(idsCollection[0], orders[0].Id);
            Assert.AreEqual(idsCollection[1], orders[1].Id);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenOrdersWithSomeOfTheSpecifiedIds_ShouldReturnThemSortedById()
        {
            var idsCollection = new List<int>() { 1, 5, 97373, 4 };

            var orders = _orderApiService.GetOrders(ids: idsCollection);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(idsCollection[0], orders[0].Id);
            Assert.AreEqual(idsCollection[3], orders[1].Id);
            Assert.AreEqual(idsCollection[1], orders[2].Id);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenOrdersThatDoNotMatchTheSpecifiedIds_ShouldReturnEmptyCollection()
        {
            var idsCollection = new List<int>() { 2123434, 5456456, 97373, -45 };

            var orders = _orderApiService.GetOrders(ids: idsCollection);

            // Assert
            CollectionAssert.IsEmpty(orders);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenEmptyIdsCollection_ShouldReturnAllNotDeletedOrders()
        {
            var idsCollection = new List<int>();

            var orders = _orderApiService.GetOrders(ids: idsCollection);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(orders.Count, _existigOrders.Count(x => !x.Deleted));
        }
    }
}