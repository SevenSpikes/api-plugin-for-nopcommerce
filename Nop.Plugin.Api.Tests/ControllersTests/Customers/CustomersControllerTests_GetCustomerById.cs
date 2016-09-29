using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Customers
{
    [TestFixture]
    public class CustomersControllerTests_GetCustomerById
    {
        [Test]
        public void WhenIdIsPositiveNumberButNoSuchCustmerWithSuchIdExists_ShouldReturn404NotFound()
        {
            int nonExistingCustomerId = 5;

            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomerById(nonExistingCustomerId)).Return(null);

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                .IgnoreArguments()
                                                .Return(string.Empty);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCustomerById(nonExistingCustomerId);

            // Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;

            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturnBadRequest(int nonPositiveCustomerId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                            .IgnoreArguments()
                                                            .Return(string.Empty);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCustomerById(nonPositiveCustomerId);

            // Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallCustomerApiService(int negativeCustomerId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            // Act
            autoMocker.ClassUnderTest.GetCustomerById(negativeCustomerId);

            // Assert
            autoMocker.Get<ICustomerApiService>().AssertWasNotCalled(x => x.GetCustomerById(negativeCustomerId));
        }

        [Test]
        public void WhenIdEqualsToExistingCustomerId_ShouldSerializeThatCustomer()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId.ToString() };
           
            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            // Act
            autoMocker.ClassUnderTest.GetCustomerById(existingCustomerId);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<CustomersRootObject>.Matches(objectToSerialize => objectToSerialize.Customers[0] == existingCustomerDto),
                Arg<string>.Matches(fields => fields == "")));
        }

        [Test]
        public void WhenIdEqualsToExistingCustomerIdAndFieldsSet_ShouldReturnJsonForThatCustomerWithSpecifiedFields()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId.ToString() };
            string fields = "id,email";

            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();
            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            // Act
            autoMocker.ClassUnderTest.GetCustomerById(existingCustomerId, fields);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<CustomersRootObject>.Matches(objectToSerialize => objectToSerialize.Customers[0] == existingCustomerDto),
                Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        }

    }
}