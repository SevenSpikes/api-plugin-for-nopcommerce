using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Plugin.Api.Tests.Helpers;

    [TestFixture]
    public class OrdersControllerTests_GetOrderById
    {
        [Test]
        public void WhenIdIsPositiveNumberButNoSuchOrderWithSuchIdExists_ShouldReturn404NotFound()
        {
            int nonExistingOrderId = 5;
            
            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();
            
            autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrderById(nonExistingOrderId)).Return(null);

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<OrdersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                       .IgnoreArguments()
                                                       .Return(string.Empty);

            // Act
            IActionResult result = autoMocker.ClassUnderTest.GetOrderById(nonExistingOrderId);

            // Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturnBadRequest(int nonPositiveOrderId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<OrdersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                     .IgnoreArguments()
                                                     .Return(string.Empty);

            // Act
            IActionResult result = autoMocker.ClassUnderTest.GetOrderById(nonPositiveOrderId);

            // Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallOrderApiService(int negativeOrderId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            // Act
            autoMocker.ClassUnderTest.GetOrderById(negativeOrderId);

            // Assert
            autoMocker.Get<IOrderApiService>().AssertWasNotCalled(x => x.GetOrderById(negativeOrderId));
        }

        [Test]
        public void WhenIdEqualsToExistingOrderId_ShouldSerializeThatOrder()
        {
            int existingOrderId = 5;
            var existingOrder = new Order() { Id = existingOrderId };
            var existingOrderDto = new OrderDto() { Id = existingOrderId.ToString() };

            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrderById(existingOrderId)).Return(existingOrder);
            autoMocker.Get<IDTOHelper>().Stub(x => x.PrepareOrderDTO(existingOrder)).Return(existingOrderDto);

            // Act
            autoMocker.ClassUnderTest.GetOrderById(existingOrderId);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<OrdersRootObject>.Matches(objectToSerialize => objectToSerialize.Orders[0].Id == existingOrderId.ToString()),
                Arg<string>.Matches(fields => fields == "")));
        }
    }
}