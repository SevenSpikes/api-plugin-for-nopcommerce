using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrders
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrders_PageParameter
    {
        private IOrderApiService _orderApiService;
        private List<Order> _existigOrders;

        [SetUp]
        public void Setup()
        {
            _existigOrders = new List<Order>();

            for (int i = 0; i < 1000; i++)
            {
                _existigOrders.Add(new Order()
                {
                    Id = i + 1
                });
            }

            _existigOrders[5].Deleted = true;
            _existigOrders = _existigOrders.OrderBy(x => x.Id).ToList();

            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(_existigOrders.AsQueryable());
            
            _orderApiService = new OrderApiService(orderRepo);
        }

        [Test]
        public void WhenCalledWithPageParameter_GivenLimitedOrdersCollection_ShouldReturnThePartOfTheCollectionThatCorrespondsToThePage()
        {
            //Arange
            var limit = 5;
            var page = 6;
            var expectedCollection = new ApiList<Order>(_existigOrders.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var orders = _orderApiService.GetOrders(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithZeroPageParameter_GivenLimitedOrdersCollection_ShouldReturnTheFirstPage()
        {
            //Arange
            var limit = 5;
            var page = 0;
            var expectedCollection = new ApiList<Order>(_existigOrders.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var orders = _orderApiService.GetOrders(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.AreEqual(_existigOrders.First().Id, orders.First().Id);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithNegativePageParameter_GivenLimitedOrdersCollection_ShouldReturnTheFirstPage()
        {
            //Arange
            var limit = 5;
            var page = -30;
            var expectedCollection = new ApiList<Order>(_existigOrders.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var orders = _orderApiService.GetOrders(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.AreEqual(_existigOrders.First().Id, orders.First().Id);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithTooBigPageParameter_GivenLimitedOrdersCollection_ShouldReturnEmptyCollection()
        {
            //Arange
            var limit = 5;
            var page = _existigOrders.Count / limit + 100;
            
            //Act
            var orders = _orderApiService.GetOrders(limit: limit, page: page);

            // Assert
            CollectionAssert.IsEmpty(orders);
        }
    }
}