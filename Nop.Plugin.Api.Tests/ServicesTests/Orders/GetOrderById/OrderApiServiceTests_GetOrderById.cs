using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrderById
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrderById
    {
        [Test]
        public void WhenNullIsReturnedByTheRepository_ShouldReturnNull()
        {
            int orderId = 3;
            
            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.Table).Return((new List<Order>()).AsQueryable());
            orderRepo.Stub(x => x.GetById(orderId)).Return(null);

            // Act  
            var cut = new OrderApiService(orderRepo);
            var result = cut.GetOrderById(orderId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(-2)]
        [TestCase(0)]
        public void WhenNegativeOrZeroOrderIdPassed_ShouldReturnNull(int negativeOrZeroOrderId)
        {
            // Aranges
            var orderRepoStub = MockRepository.GenerateStub<IRepository<Order>>();

            // Act
            var cut = new OrderApiService(orderRepoStub);
            var result = cut.GetOrderById(negativeOrZeroOrderId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void WhenOrderIsReturnedByTheRepository_ShouldReturnTheSameOrder()
        {
            int orderId = 3;
            var order = new Order() { Id = 3 };

            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();

            var list = new List<Order>();
            list.Add(order);

            orderRepo.Stub(x => x.Table).Return(list.AsQueryable());
            orderRepo.Stub(x => x.GetById(orderId)).Return(order);
            
            // Act
            var cut = new OrderApiService(orderRepo);
            var result = cut.GetOrderById(orderId);

            // Assert
            Assert.AreSame(order, result);
        }
    }
}
