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
    public class OrderApiServiceTests_GetOrdersCount_DefaultParameters
    {
        [Test]
        public void WhenCalledWithDefaultParameters_GivenNoOrdersExist_ShouldReturnZero()
        {
            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(new List<Order>().AsQueryable());
            
            // Act
            var cut = new OrderApiService(orderRepo);
            var ordersCount = cut.GetOrdersCount();

            // Assert
            Assert.AreEqual(0, ordersCount);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenOnlyDeletedOrdersExist_ShouldReturnZero()
        {
            var existingOrders = new List<Order>();
            existingOrders.Add(new Order() { Id = 1, Deleted = true });
            existingOrders.Add(new Order() { Id = 2, Deleted = true });

            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(existingOrders.AsQueryable());
            
            // Act
            var cut = new OrderApiService(orderRepo);
            var countResult = cut.GetOrdersCount();

            // Assert
            Assert.AreEqual(0, countResult);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeNotDeletedOrderExist_ShouldReturnTheirCount()
        {
            var existingOrders = new List<Order>();
            existingOrders.Add(new Order() { Id = 1 });
            existingOrders.Add(new Order() { Id = 2, Deleted = true });
            existingOrders.Add(new Order() { Id = 3 });

            // Arange
            var ordersRepo = MockRepository.GenerateStub<IRepository<Order>>();
            ordersRepo.Stub(x => x.TableNoTracking).Return(existingOrders.AsQueryable());
            
            // Act
            var cut = new OrderApiService(ordersRepo);
            var countResult = cut.GetOrdersCount();

            // Assert
            Assert.AreEqual(2, countResult);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeOrdersExist_ShouldReturnTheirCount()
        {
            var existingOrders = new List<Order>();
            existingOrders.Add(new Order() { Id = 2 });
            existingOrders.Add(new Order() { Id = 3 });
            existingOrders.Add(new Order() { Id = 1 });

            // Arange
            var ordersRepo = MockRepository.GenerateStub<IRepository<Order>>();
            ordersRepo.Stub(x => x.TableNoTracking).Return(existingOrders.AsQueryable());

            // Act
            var cut = new OrderApiService(ordersRepo);
            var countResult = cut.GetOrdersCount();

            // Assert
            Assert.AreEqual(existingOrders.Count, countResult);
        }
    }
}