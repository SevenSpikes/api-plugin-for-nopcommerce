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
    public class OrderApiServiceTests_GetOrders_SinceIdParameter
    {
        private IOrderApiService _orderApiService;
        private List<Order> _existigOrders;

        [SetUp]
        public void Setup()
        {
            _existigOrders = new List<Order>()
            {
                new Order() {Id = 2 },
                new Order() {Id = 3 },
                new Order() {Id = 1 },
                new Order() {Id = 4 },
                new Order() {Id = 5 },
                new Order() {Id = 6, Deleted = true },
                new Order() {Id = 7 }
            };

            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(_existigOrders.AsQueryable());
            
            _orderApiService = new OrderApiService(orderRepo);
        }

        [Test]
        public void WhenCalledWithValidSinceId_ShouldReturnOnlyTheOrdersAfterThisIdSortedById()
        {
            // Arange
            int sinceId = 3;
            var expectedCollection = _existigOrders.Where(x => x.Id > sinceId && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var orders = _orderApiService.GetOrders(sinceId: sinceId);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledZeroOrNegativeSinceId_ShouldReturnAllTheOrdersSortedById(int sinceId)
        {
            // Arange
            var expectedCollection = _existigOrders.Where(x => x.Id > sinceId && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var orders = _orderApiService.GetOrders(sinceId: sinceId);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledSinceIdOutsideOfTheOrdersIdsRange_ShouldReturnEmptyCollection()
        {
            // Arange
            int sinceId = int.MaxValue;
        
            // Act
            var orders = _orderApiService.GetOrders(sinceId: sinceId);
            
            // Assert
            CollectionAssert.IsEmpty(orders);
        }
    }
}