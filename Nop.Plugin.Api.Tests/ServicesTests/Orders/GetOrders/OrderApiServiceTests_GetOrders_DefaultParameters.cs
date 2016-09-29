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
    public class OrderApiServiceTests_GetOrders_DefaultParameters
    {
        [Test]
        public void WhenCalledWithDefaultParameters_GivenNoOrdersExist_ShouldReturnEmptyCollection()
        {
            // Arange
            var ordersRepo = MockRepository.GenerateStub<IRepository<Order>>();
            ordersRepo.Stub(x => x.TableNoTracking).Return(new List<Order>().AsQueryable());
            
            // Act
            var cut = new OrderApiService(ordersRepo);
            var orders = cut.GetOrders();

            // Assert
            CollectionAssert.IsEmpty(orders);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenOnlyDeletedOrdersExist_ShouldReturnEmptyCollection()
        {
            var existingOrders = new List<Order>();
            existingOrders.Add(new Order() { Id = 1, Deleted = true });
            existingOrders.Add(new Order() { Id = 2, Deleted = true });

            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(existingOrders.AsQueryable());
            
            // Act
            var cut = new OrderApiService(orderRepo);
            var orders = cut.GetOrders();

            // Assert
            CollectionAssert.IsEmpty(orders);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeNotDeletedOrdersExist_ShouldReturnThemSortedById()
        {
            var existingOrders = new List<Order>();
            existingOrders.Add(new Order() { Id = 1 });
            existingOrders.Add(new Order() { Id = 2, Deleted = true });
            existingOrders.Add(new Order() { Id = 3 });

            var expectedCollection = existingOrders.Where(x => !x.Deleted).OrderBy(x => x.Id);

            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(existingOrders.AsQueryable());
            
            // Act
            var cut = new OrderApiService(orderRepo);
            var orders = cut.GetOrders();

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeOrdersExist_ShouldReturnThemSortedById()
        {
            var existingOrders = new List<Order>();
            existingOrders.Add(new Order() { Id = 2 });
            existingOrders.Add(new Order() { Id = 3 });
            existingOrders.Add(new Order() { Id = 1 });

            var expectedCollection = existingOrders.Where(x => !x.Deleted).OrderBy(x => x.Id);

            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(existingOrders.AsQueryable());
            
            // Act
            var cut = new OrderApiService(orderRepo);
            var orders = cut.GetOrders();

            // Assert
            CollectionAssert.IsNotEmpty(orders);
            Assert.AreEqual(expectedCollection.Count(), orders.Count);
            Assert.IsTrue(orders.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }
    }
}