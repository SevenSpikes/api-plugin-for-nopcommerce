using System.Collections.Generic;
using System.Net;
using AutoMock;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Stores;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Plugin.Api.JSON.Serializers;
    using Nop.Plugin.Api.Tests.Helpers;

    [TestFixture]
    public class CategoriesControllerTests_GetCategories
    {
        private RhinoAutoMocker<CategoriesController> _authMocker;

        [SetUp]
        public void Setup()
        {
            _authMocker = new RhinoAutoMocker<CategoriesController>();

            _authMocker.Get<IStoreMappingService>().Stub(x => x.Authorize(Arg<Category>.Is.Anything)).Return(true);

            _authMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Anything))
                .IgnoreArguments()
                .Return(string.Empty);
        }

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
            _authMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                           .IgnoreArguments()
                                                           .Return(string.Empty);

            //Act
            IActionResult result = _authMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

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
            _authMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                           .IgnoreArguments()
                                                           .Return(string.Empty);

            //Act
            IActionResult result = _authMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new CategoriesParametersModel();

            //Arange
            _authMocker.Get<ICategoryApiService>()
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
            _authMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            _authMocker.Get<ICategoryApiService>().VerifyAllExpectations();
        }

        // The static method category.GetSeName() breaks this test as we can't stub static methods :(
        //[Test]
        //public void WhenSomeCategoriesExist_ShouldCallTheSerializerWithTheseCategories()
        //{
        //    MappingExtensions.Maps.CreateMap<Category, CategoryDto>();

        //    var returnedCategoriesCollection = new List<Category>()
        //    {
        //        new Category(),
        //        new Category()
        //    };

        //    var parameters = new CategoriesParametersModel();

        //    //Arange
        //    var autoMocker = new RhinoAutoMocker<CategoriesController>();
        //    autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategories()).Return(returnedCategoriesCollection);
        //    autoMocker.Get<IAclService>().Stub(x => x.GetAclRecords(new Category())).IgnoreArguments().Return(new List<AclRecord>());
        //    autoMocker.Get<IStoreMappingService>().Stub(x => x.GetStoreMappings(new Category())).IgnoreArguments().Return(new List<StoreMapping>());

        //    //Act
        //    autoMocker.ClassUnderTest.GetCategories(parameters);

        //    //Assert
        //    autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
        //        x => x.Serialize(Arg<CategoriesRootObject>.Matches(r => r.Categories.Count == 2),
        //        Arg<string>.Is.Equal(parameters.Fields)));
        //}

        [Test]
        public void WhenAnyFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new CategoriesParametersModel()
            {
                Fields = "id,name"
            };

            //Arange
            _authMocker.Get<ICategoryApiService>().Stub(x => x.GetCategories()).Return(new List<Category>());

            //Act
            _authMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            _authMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenNoCategoriesExist_ShouldCallTheSerializerWithRootObjectWithoutCategories()
        {
            var parameters = new CategoriesParametersModel();

            //Arange
            _authMocker.Get<ICategoryApiService>().Stub(x => x.GetCategories()).Return(new List<Category>());

            //Act
            _authMocker.ClassUnderTest.GetCategories(parameters);

            //Assert
            _authMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Matches(r => r.Categories.Count == 0),
                Arg<string>.Is.Equal(parameters.Fields)));
        }
    }
}