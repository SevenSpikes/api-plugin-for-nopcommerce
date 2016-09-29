using System.Collections.Generic;
using AutoMock;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    [TestFixture]
    public class OrdersControllerTests_GetOrdersByCustomerId
    {
        [Test]
        [TestCase(-5)]
        [TestCase(0)]
        [TestCase(10)]
        [TestCase(int.MaxValue)]
        public void WhenCustomerIdIsPassed_ShouldCallTheServiceWithThePassedParameters(int customerId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IOrderApiService>().Expect(x => x.GetOrdersByCustomerId(customerId)).Return(new List<Order>());

            // Act
            autoMocker.ClassUnderTest.GetOrdersByCustomerId(customerId);

            // Assert
            autoMocker.Get<IOrderApiService>().VerifyAllExpectations();
        }
    }
}