using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.ProductCategoryMappingsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.ProductCategoryMappings
{
    [TestFixture]
    public class ProductCategoryMappingsControllerTests_GetMappings
    {
        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1 // some different than default limit
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IProductCategoryMappingsApiService>().Expect(x => x.GetMappings(parameters.ProductId,
                                                                           parameters.CategoryId,
                                                                           parameters.Limit,
                                                                           parameters.Page,
                                                                           parameters.SinceId)).Return(new List<ProductCategory>());

            //Act
            autoMocker.ClassUnderTest.GetMappings(parameters);

            //Assert
            autoMocker.Get<IProductCategoryMappingsApiService>().VerifyAllExpectations();
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                 .IgnoreArguments()
                                                 .Return(string.Empty);

            //Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetMappings(parameters);

            //Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                .IgnoreArguments()
                                                .Return(string.Empty);

            //Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetMappings(parameters);

            //Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        public void WhenNoProductCategoryMappingsExist_ShouldCallTheSerializerWithNoProductCategoryMappings()
        {
            var returnedMappingsCollection = new List<ProductCategory>();

            var parameters = new ProductCategoryMappingsParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IProductCategoryMappingsApiService>().Stub(x => x.GetMappings()).IgnoreArguments().Return(returnedMappingsCollection);

            //Act
            autoMocker.ClassUnderTest.GetMappings(parameters);

            //Assert
            autoMocker.Get< IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Matches(r => r.ProductCategoryMappingDtos.Count == returnedMappingsCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                Fields = "id,product_id"
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IProductCategoryMappingsApiService>().Stub(x => x.GetMappings()).Return(new List<ProductCategory>());

            //Act
            autoMocker.ClassUnderTest.GetMappings(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenSomeProductCategoryMappingsExist_ShouldCallTheSerializerWithTheseProductCategoryMappings()
        {
            MappingExtensions.Maps.CreateMap<ProductCategory, ProductCategoryMappingDto>();

            var returnedMappingsDtoCollection = new List<ProductCategory>()
            {
                new ProductCategory(),
                new ProductCategory()
            };

            var parameters = new ProductCategoryMappingsParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<ProductCategoryMappingsController>();

            autoMocker.Get<IProductCategoryMappingsApiService>().Stub(x => x.GetMappings()).Return(returnedMappingsDtoCollection);

            //Act
            autoMocker.ClassUnderTest.GetMappings(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Matches(r => r.ProductCategoryMappingDtos.Count == returnedMappingsDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }
    }
}