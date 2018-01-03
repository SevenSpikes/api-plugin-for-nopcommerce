using System;
using AutoMock;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Core;
    using Nop.Core.Domain.Stores;
    using Nop.Plugin.Api.DTOs.Orders;
    using Nop.Plugin.Api.JSON.Serializers;

    [TestFixture]
    public class OrdersControllerTests_GetOrdersCount
    {
        private RhinoAutoMocker<OrdersController> _autoMocker;

        [SetUp]
        public void StartUp()
        {
            _autoMocker = new RhinoAutoMocker<OrdersController>();
            _autoMocker.Get<IStoreContext>().Stub(x => x.CurrentStore).Return(new Store());
        }

        [Test]
        public void WhenNoOrdersExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();

            _autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            _autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(0);

            // act
            IActionResult result = _autoMocker.ClassUnderTest.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(0, ((OrdersCountRootObject)((OkObjectResult)result).Value).Count);
        }

        [Test]
        public void WhenSingleOrderExists_ShouldReturnOKWithCountEqualToOne()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();

            _autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            _autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(1);

            // act
            IActionResult result = _autoMocker.ClassUnderTest.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(1, ((OrdersCountRootObject)((OkObjectResult)result).Value).Count);
        }

        [Test]
        public void WhenCertainNumberOfOrdersExist_ShouldReturnOKWithCountEqualToSameNumberOfOrders()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();

            _autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            _autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(20000);

            // act
            IActionResult result = _autoMocker.ClassUnderTest.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(20000, ((OrdersCountRootObject)((OkObjectResult)result).Value).Count);
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
            _autoMocker.Get<IOrderApiService>().Expect(x => x.GetOrdersCount(parameters.CreatedAtMin,
                                                            parameters.CreatedAtMax,
                                                            parameters.Status,
                                                            parameters.PaymentStatus,
                                                            parameters.ShippingStatus,
                                                            parameters.CustomerId)).IgnoreArguments().Return(1);

            //Act
            _autoMocker.ClassUnderTest.GetOrdersCount(parameters);

            //Assert
            _autoMocker.Get<IOrderApiService>().VerifyAllExpectations();
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
            _autoMocker.Get<IOrderApiService>().Expect(x => x.GetOrdersCount(parameters.CreatedAtMin,
                                                            parameters.CreatedAtMax,
                                                            parameters.Status,
                                                            parameters.PaymentStatus,
                                                            parameters.ShippingStatus,
                                                            parameters.CustomerId)).IgnoreArguments().Return(0);

            //Act
            _autoMocker.ClassUnderTest.GetOrdersCount(parameters);

            //Assert
            _autoMocker.Get<IOrderApiService>().VerifyAllExpectations();
        }
    }
}