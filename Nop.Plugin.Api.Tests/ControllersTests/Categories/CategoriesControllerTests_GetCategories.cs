using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Security;
using Nop.Services.Stores;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    [TestFixture]
    public class CategoriesControllerTests_GetCategories
    {
        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new CategoriesParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                           .IgnoreArguments()
                                                           .Return(string.Empty);

            //Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;
                
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new CategoriesParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                           .IgnoreArguments()
                                                           .Return(string.Empty);

            //Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new CategoriesParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<ICategoryApiService>()
                .Expect(x => x.GetCategories(parameters.Ids,
                                                    parameters.CreatedAtMin,
                                                    parameters.CreatedAtMax,
                                                    parameters.UpdatedAtMin,
                                                    parameters.UpdatedAtMax,
                                                    parameters.Limit,
                                                    parameters.Page,
                                                    parameters.SinceId,
                                                    parameters.ProductId,
                                                    parameters.PublishedStatus)).Return(new List<Category>());

            //Act
            autoMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            autoMocker.Get<ICategoryApiService>().VerifyAllExpectations();
        }

        [Test]
        public void WhenSomeCategoriesExist_ShouldCallTheSerializerWithTheseCategories()
        {
            MappingExtensions.Maps.CreateMap<Category, CategoryDto>();

            var returnedCategoriesCollection = new List<Category>()
            {
                new Category(),
                new Category()
            };

            var parameters = new CategoriesParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategories()).Return(returnedCategoriesCollection);
            autoMocker.Get<IAclService>().Stub(x => x.GetAclRecords(new Category())).IgnoreArguments().Return(new List<AclRecord>());
            autoMocker.Get<IStoreMappingService>().Stub(x => x.GetStoreMappings(new Category())).IgnoreArguments().Return(new List<StoreMapping>());

            //Act
            autoMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Matches(r => r.Categories.Count == 2),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenAnyFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new CategoriesParametersModel()
            {
                Fields = "id,name"
            };

            //Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategories()).Return(new List<Category>());

            //Act
            autoMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenNoCategoriesExist_ShouldCallTheSerializerWithRootObjectWithoutCategories()
        {
            var parameters = new CategoriesParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategories()).Return(new List<Category>());

            //Act
            autoMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Matches(r => r.Categories.Count == 0),
                Arg<string>.Is.Equal(parameters.Fields)));
        }


    }
}