using AutoMock;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    using Microsoft.AspNetCore.Mvc;

    [TestFixture]
    public class CategoriesControllerTests_GetCategoriesCount
    {
        [Test]
        public void WhenNoCategoriesExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            var parameters = new CategoriesCountParametersModel();

            // arrange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(0);

            //  act
            IActionResult result = autoMocker.ClassUnderTest.GetCategoriesCount(parameters);

            // assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okObjectResult = result as OkObjectResult;

            Assert.NotNull(okObjectResult);

            Assert.IsInstanceOf<CategoriesCountRootObject>(okObjectResult.Value);

            var rootObject = okObjectResult.Value as CategoriesCountRootObject;

            Assert.NotNull(rootObject);

            Assert.AreEqual(0, rootObject.Count);
        }

        [Test]
        public void WhenSingleCategoryExists_ShouldReturnOKWithCountEqualToOne()
        {
            var parameters = new CategoriesCountParametersModel();

            // arrange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(1);

            // act
            IActionResult result = autoMocker.ClassUnderTest.GetCategoriesCount(parameters);

            // assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okObjectResult = result as OkObjectResult;

            Assert.NotNull(okObjectResult);

            Assert.IsInstanceOf<CategoriesCountRootObject>(okObjectResult.Value);

            var rootObject = okObjectResult.Value as CategoriesCountRootObject;

            Assert.NotNull(rootObject);

            Assert.AreEqual(1, rootObject.Count);
        }

        [Test]
        public void WhenCertainNumberOfCategoriesExist_ShouldReturnOKWithCountEqualToSameNumberOfCategories()
        {
            var categoriesCountParametersModel = new CategoriesCountParametersModel();
            int categoriesCount = 20;

            // arrange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(categoriesCount);

            // act
            IActionResult result = autoMocker.ClassUnderTest.GetCategoriesCount(categoriesCountParametersModel);

            // assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okObjectResult = result as OkObjectResult;

            Assert.NotNull(okObjectResult);

            Assert.IsInstanceOf<CategoriesCountRootObject>(okObjectResult.Value);

            var rootObject = okObjectResult.Value as CategoriesCountRootObject;

            Assert.NotNull(rootObject);

            Assert.AreEqual(categoriesCount, rootObject.Count);
        }
    }
}