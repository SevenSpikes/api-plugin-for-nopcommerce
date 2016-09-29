using System;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    [TestFixture]
    public class OrdersControllerTests_GetOrdersCount
    {
        [Test]
        public void WhenNoOrdersExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();
            
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(0);

            // act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<OrdersCountRootObject>>(result);
            Assert.AreEqual(0, ((OkNegotiatedContentResult<OrdersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSingleOrderExists_ShouldReturnOKWithCountEqualToOne()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();
            
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(1);

            // act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<OrdersCountRootObject>>(result);
            Assert.AreEqual(1, ((OkNegotiatedContentResult<OrdersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenCertainNumberOfOrdersExist_ShouldReturnOKWithCountEqualToSameNumberOfOrders()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();
            
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(20000);

            // act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<OrdersCountRootObject>>(result);
            Assert.AreEqual(20000, ((OkNegotiatedContentResult<OrdersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new OrdersCountParametersModel()
            {
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Status = OrderStatus.Complete,
                ShippingStatus = ShippingStatus.Delivered,
                PaymentStatus = PaymentStatus.Authorized,
                CustomerId = 10
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IOrderApiService>().Expect(x => x.GetOrdersCount(parameters.CreatedAtMin,
                                                            parameters.CreatedAtMax,
                                                            parameters.Status,
                                                            parameters.PaymentStatus,
                                                            parameters.ShippingStatus,
                                                            parameters.CustomerId)).Return(1);

            //Act
            autoMocker.ClassUnderTest.GetOrdersCount(parameters);

            //Assert
            autoMocker.Get<IOrderApiService>().VerifyAllExpectations();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidCustomerIdParameterPassed_ShouldCallTheServiceWithThisCustomerId(int invalidCustomerId)
        {
            var parameters = new OrdersCountParametersModel()
            {
                CustomerId = invalidCustomerId
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IOrderApiService>().Expect(x => x.GetOrdersCount(parameters.CreatedAtMin,
                                                            parameters.CreatedAtMax,
                                                            parameters.Status,
                                                            parameters.PaymentStatus,
                                                            parameters.ShippingStatus,
                                                            parameters.CustomerId)).Return(0);

            //Act
            autoMocker.ClassUnderTest.GetOrdersCount(parameters);

            //Assert
            autoMocker.Get<IOrderApiService>().VerifyAllExpectations();
        }
    }
}