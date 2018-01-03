//using System.Collections.Generic;
//using AutoMock;
//using Nop.Plugin.Api.Controllers;
//using NUnit.Framework;

//namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
//{
//    using Microsoft.AspNetCore.Mvc;

//    [TestFixture]
//    public class CategoriesControllerTests_CreateCategory
//    {
//        [Test]
//        public void WhenCategoryRootParameterIsNull_ShouldReturnBadRequest()
//        {
//            // Arrange
//            var autoMocker = new RhinoAutoMocker<CategoriesController>();

//            // Act
//            IActionResult result = autoMocker.ClassUnderTest.CreateCategory(null);

//            // Assert
//            Assert.IsInstanceOf<BadRequestObjectResult>(result);
//        }

//        [Test]
//        public void WhenCategoryRootParameterIsEmpty_ShouldReturnBadRequest()
//        {
//            // Arrange
//            var autoMocker = new RhinoAutoMocker<CategoriesController>();

//            // Act
//            IActionResult result = autoMocker.ClassUnderTest.CreateCategory(new Dictionary<string, object>());

//            // Assert
//            Assert.IsInstanceOf<BadRequestObjectResult>(result);
//        }

//        [Test]
//        public void WhenCategoryRootParameterDoesNotContainCategoryObjectOnRootLevel_ShouldReturnBadRequest()
//        {
//            // Arrange
//            var autoMocker = new RhinoAutoMocker<CategoriesController>();

//            // Act
//            IActionResult result = autoMocker.ClassUnderTest.CreateCategory(new Dictionary<string, object>()
//            {
//                {
//                    "this should be category",
//                    "collection of property-values"
//                }
//            });

//            // Assert
//            Assert.IsInstanceOf<BadRequestObjectResult>(result);
//        }

//        // could not test anything with valid object because of the static extension methods that we are required to use when creating a new category
//        // and the limitation of Rhino Mocks.
//    }
//}