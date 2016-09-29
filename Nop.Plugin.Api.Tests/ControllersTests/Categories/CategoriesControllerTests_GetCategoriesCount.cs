using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    [TestFixture]
    public class CategoriesControllerTests_GetCategoriesCount
    {
        [Test]
        public void WhenNoCategoriesExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            var parameters = new CategoriesCountParametersModel();

            // arrange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(0);

            //  act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCategoriesCount(parameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CategoriesCountRootObject>>(result);
            Assert.AreEqual(0, ((OkNegotiatedContentResult<CategoriesCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSingleCategoryExists_ShouldReturnOKWithCountEqualToOne()
        {
            var parameters = new CategoriesCountParametersModel();

            // arrange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(1);

            // act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCategoriesCount(parameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CategoriesCountRootObject>>(result);
            Assert.AreEqual(1, ((OkNegotiatedContentResult<CategoriesCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenCertainNumberOfCategoriesExist_ShouldReturnOKWithCountEqualToSameNumberOfCategories()
        {
            var categoriesCountParametersModel = new CategoriesCountParametersModel();
            int categoriesCount = 20;

            // arrange
            var autoMocker = new RhinoAutoMocker<CategoriesApiController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(categoriesCount);

            // act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCategoriesCount(categoriesCountParametersModel);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CategoriesCountRootObject>>(result);
            Assert.AreEqual(categoriesCount, ((OkNegotiatedContentResult<CategoriesCountRootObject>)result).Content.Count);
        }
    }
}