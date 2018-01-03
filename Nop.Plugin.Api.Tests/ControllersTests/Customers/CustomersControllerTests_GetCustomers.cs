using System;
using System.Collections.Generic;
using System.Net;
using AutoMock;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Customers
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Plugin.Api.JSON.Serializers;
    using Nop.Plugin.Api.Tests.Helpers;

    [TestFixture]
    public class CustomersControllerTests_GetCustomers
    {
        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new CustomersParametersModel()
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1 // some different than default limit
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            //Act
            autoMocker.ClassUnderTest.GetCustomers(parameters);

            //Assert
            autoMocker.Get<ICustomerApiService>().AssertWasCalled(x => x.GetCustomersDtos(parameters.CreatedAtMin,
                                                    parameters.CreatedAtMax,
                                                    parameters.Limit,
                                                    parameters.Page,
                                                    parameters.SinceId));
        }

        [Test]
        public void WhenSomeCustomersExist_ShouldCallTheSerializerWithTheseCustomers()
        {
            var returnedCustomersDtoCollection = new List<CustomerDto>()
            {
                new CustomerDto(),
                new CustomerDto()
            };

            var parameters = new CustomersParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomersDtos()).Return(returnedCustomersDtoCollection);

            //Act
            autoMocker.ClassUnderTest.GetCustomers(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Matches(r => r.Customers.Count == returnedCustomersDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenNoCustomersExist_ShouldCallTheSerializerWithNoCustomers()
        {
            var returnedCustomersDtoCollection = new List<CustomerDto>();

            var parameters = new CustomersParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomersDtos()).Return(returnedCustomersDtoCollection);

            //Act
            autoMocker.ClassUnderTest.GetCustomers(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Matches(r => r.Customers.Count == returnedCustomersDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new CustomersParametersModel()
            {
                Fields = "id,email"
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            //Act
            autoMocker.ClassUnderTest.GetCustomers(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new CustomersParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                                        .IgnoreArguments()
                                                                        .Return(string.Empty);

            //Act
            IActionResult result = autoMocker.ClassUnderTest.GetCustomers(parameters);

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new CustomersParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                                        .IgnoreArguments()
                                                                        .Return(string.Empty);

            //Act
            IActionResult result = autoMocker.ClassUnderTest.GetCustomers(parameters);

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }
    }
}