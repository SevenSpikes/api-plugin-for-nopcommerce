using System.Net;
using AutoMock;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.ProductCategoryMappings
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Plugin.Api.JSON.Serializers;
    using Nop.Plugin.Api.Tests.Helpers;

    [TestFixture]
    public class ProductCategoryMappingsControllerTests_GetMappingById
    {
        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturnBadRequest(int nonPositiveProductCategoryMappingId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                   .IgnoreArguments()
                                                   .Return(string.Empty);

            // Act
            IActionResult result = autoMocker.ClassUnderTest.GetMappingById(nonPositiveProductCategoryMappingId);

            // Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallProductCategoryMappingsApiService(int nonPositiveProductCategoryMappingId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);

            // Act
            autoMocker.ClassUnderTest.GetMappingById(nonPositiveProductCategoryMappingId);

            // Assert
            autoMocker.Get<IProductCategoryMappingsApiService>().AssertWasNotCalled(x => x.GetById(nonPositiveProductCategoryMappingId));
        }

        [Test]
        public void WhenIdIsPositiveNumberButNoSuchMappingExists_ShouldReturn404NotFound()
        {
            int nonExistingMappingId = 5;

            // Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IProductCategoryMappingsApiService>().Stub(x => x.GetById(nonExistingMappingId)).Return(null);

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                   .IgnoreArguments()
                                                   .Return(string.Empty);

            // Act
            IActionResult result = autoMocker.ClassUnderTest.GetMappingById(nonExistingMappingId);

            // Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }

        [Test]
        public void WhenIdEqualsToExistingMappingId_ShouldSerializeThatMapping()
        {
            //MappingExtensions.Maps.CreateMap<ProductCategory, ProductCategoryMappingDto>();

            int existingMappingId = 5;
            var existingMapping = new ProductCategory() { Id = existingMappingId };

            // Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IProductCategoryMappingsApiService>().Stub(x => x.GetById(existingMappingId)).Return(existingMapping);

            // Act
            autoMocker.ClassUnderTest.GetMappingById(existingMappingId);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<ProductCategoryMappingsRootObject>.Matches(
                        objectToSerialize =>
                               objectToSerialize.ProductCategoryMappingDtos.Count == 1 &&
                               objectToSerialize.ProductCategoryMappingDtos[0].Id == existingMapping.Id),
                    Arg<string>.Is.Equal("")));
        }

        [Test]
        public void WhenIdEqualsToExistingProductCategoryMappingIdAndFieldsSet_ShouldReturnJsonForThatProductCategoryMappingWithSpecifiedFields()
        {
            //MappingExtensions.Maps.CreateMap<ProductCategory, ProductCategoryMappingDto>();

            int existingMappingId = 5;
            var existingMapping = new ProductCategory() { Id = existingMappingId };
            string fields = "id,name";

            // Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IProductCategoryMappingsApiService>().Stub(x => x.GetById(existingMappingId)).Return(existingMapping);

            // Act
            autoMocker.ClassUnderTest.GetMappingById(existingMappingId, fields);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<ProductCategoryMappingsRootObject>.Matches(objectToSerialize => objectToSerialize.ProductCategoryMappingDtos[0].Id == existingMapping.Id),
                    Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        }
    }
}