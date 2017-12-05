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
    using Microsoft.AspNetCore.Mvc;

    [TestFixture]
    public class CustomersControllerTests_GetCustomersCount
    {
        [Test]
        public void WhenNoCustomersExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            // arrange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomersCount()).Return(0);

            // act
            IActionResult result = autoMocker.ClassUnderTest.GetCustomersCount();

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersCountRootObject>>(result);
            Assert.AreEqual(0,((OkNegotiatedContentResult<CustomersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSingleCustomerExists_ShouldReturnOKWithCountEqualToOne()
        {
            // arrange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);
            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomersCount()).Return(1);

            // act
            IActionResult result = autoMocker.ClassUnderTest.GetCustomersCount();

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersCountRootObject>>(result);
            Assert.AreEqual(1, ((OkNegotiatedContentResult<CustomersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenCertainNumberOfCustomersExist_ShouldReturnOKWithCountEqualToSameNumberOfCustomers()
        {
            // arrange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomersCount()).Return(20000);
            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);

            // act
            IActionResult result = autoMocker.ClassUnderTest.GetCustomersCount();
            
            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersCountRootObject>>(result);
            Assert.AreEqual(20000, ((OkNegotiatedContentResult<CustomersCountRootObject>)result).Content.Count);
        }
    }
}