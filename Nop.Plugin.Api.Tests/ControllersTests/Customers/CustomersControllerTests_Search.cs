using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Customers
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Plugin.Api.Tests.Helpers;

    [TestFixture]
    public class CustomersControllerTests_Search
    {
        [Test]
        public void WhenNoParametersPassed_ShouldCallTheServiceWithDefaultParameters()
        {
            var defaultParametersModel = new CustomersSearchParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            //Act
            autoMocker.ClassUnderTest.Search(defaultParametersModel);

            //Assert
            autoMocker.Get<ICustomerApiService>().AssertWasCalled(x => x.Search(defaultParametersModel.Query,
                                                    defaultParametersModel.Order,
                                                    defaultParametersModel.Page,
                                                    defaultParametersModel.Limit));
        }

        [Test]
        public void WhenNoParametersPassedAndSomeCustomersExist_ShouldCallTheSerializer()
        {
            var expectedCustomersCollection = new List<CustomerDto>()
            {
                new CustomerDto(),
                new CustomerDto()
            };

            var expectedRootObject = new CustomersRootObject()
            {
                Customers = expectedCustomersCollection
            };

            var defaultParameters = new CustomersSearchParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Expect(x => x.Serialize(expectedRootObject, defaultParameters.Fields));
            autoMocker.Get<ICustomerApiService>().Stub(x => x.Search()).Return(expectedCustomersCollection);

            //Act
            autoMocker.ClassUnderTest.Search(defaultParameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(x => x.Serialize(Arg<CustomersRootObject>.Is.TypeOf,
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenNoParametersPassedAndNoCustomersExist_ShouldCallTheSerializer()
        {
            var expectedCustomersCollection = new List<CustomerDto>();

            var expectedRootObject = new CustomersRootObject()
            {
                Customers = expectedCustomersCollection
            };

            var defaultParameters = new CustomersSearchParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Expect(x => x.Serialize(expectedRootObject, defaultParameters.Fields));
            autoMocker.Get<ICustomerApiService>().Stub(x => x.Search()).Return(expectedCustomersCollection);

            //Act
            autoMocker.ClassUnderTest.Search(defaultParameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(x => x.Serialize(Arg<CustomersRootObject>.Is.TypeOf,
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var defaultParametersModel = new CustomersSearchParametersModel()
            {
                Fields = "id,email"
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            //Act
            autoMocker.ClassUnderTest.Search(defaultParametersModel);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Equal(defaultParametersModel.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit)]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parametersModel = new CustomersSearchParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                                        .IgnoreArguments()
                                                                        .Return(string.Empty);

            //Act
            IActionResult result = autoMocker.ClassUnderTest.Search(parametersModel);

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenNonPositivePagePassed_ShouldReturnBadRequest(int nonPositivePage)
        {
            var parametersModel = new CustomersSearchParametersModel()
            {
                Limit = nonPositivePage
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                                        .IgnoreArguments()
                                                                        .Return(string.Empty);

            //Act
            IActionResult result = autoMocker.ClassUnderTest.Search(parametersModel);

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }
    }
}