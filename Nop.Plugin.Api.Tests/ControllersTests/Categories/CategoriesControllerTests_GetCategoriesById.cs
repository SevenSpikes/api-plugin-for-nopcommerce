using System.Net;
using AutoMock;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    using Microsoft.AspNetCore.Mvc;
    using Nop.Plugin.Api.JSON.Serializers;
    using Nop.Plugin.Api.Tests.Helpers;

    [TestFixture]
    public class CategoriesControllerTests_GetCategoriesById
    {
        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturnBadRequest(int nonPositiveCategoryId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                           .IgnoreArguments()
                                                           .Return(string.Empty);

            // Act
            IActionResult result = autoMocker.ClassUnderTest.GetCategoryById(nonPositiveCategoryId);

            // Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallCategoryApiService(int negativeCategoryId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();
            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);

            // Act
            autoMocker.ClassUnderTest.GetCategoryById(negativeCategoryId);

            // Assert
            autoMocker.Get<ICategoryApiService>().AssertWasNotCalled(x => x.GetCategoryById(negativeCategoryId));
        }

        [Test]
        public void WhenIdIsPositiveNumberButNoSuchCategoryExists_ShouldReturn404NotFound()
        {
            int nonExistingCategoryId = 5;

            // Arange
            var autoMocker = new RhinoAutoMocker<CategoriesController>();

            autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoryById(nonExistingCategoryId)).Return(null);

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                           .IgnoreArguments()
                                                           .Return(string.Empty);

            // Act
            IActionResult result = autoMocker.ClassUnderTest.GetCategoryById(nonExistingCategoryId);

            // Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }

        // The static method category.GetSeName() breaks this test as we can't stub static methods :(
        //[Test]
        //public void WhenIdEqualsToExistingCategoryId_ShouldSerializeThatCategory()
        //{
        //    MappingExtensions.Maps.CreateMap<Category, CategoryDto>();

        //    int existingCategoryId = 5;
        //    var existingCategory = new Category() { Id = existingCategoryId };

        //    // Arange
        //    var autoMocker = new RhinoAutoMocker<CategoriesController>();
        //    autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoryById(existingCategoryId)).Return(existingCategory);
        //    autoMocker.Get<IAclService>().Stub(x => x.GetAclRecords(new Category())).IgnoreArguments().Return(new List<AclRecord>());
        //    autoMocker.Get<IStoreMappingService>().Stub(x => x.GetStoreMappings(new Category())).IgnoreArguments().Return(new List<StoreMapping>());

        //    // Act
        //    autoMocker.ClassUnderTest.GetCategoryById(existingCategoryId);

        //    // Assert
        //    autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
        //        x => x.Serialize(
        //            Arg<CategoriesRootObject>.Matches(
        //                objectToSerialize =>
        //                       objectToSerialize.Categories.Count == 1 &&
        //                       objectToSerialize.Categories[0].Id == existingCategory.Id.ToString() &&
        //                       objectToSerialize.Categories[0].Name == existingCategory.Name),
        //            Arg<string>.Is.Equal("")));
        //}

        // The static method category.GetSeName() breaks this test as we can't stub static methods
        //[Test]
        //public void WhenIdEqualsToExistingCategoryIdAndFieldsSet_ShouldReturnJsonForThatCategoryWithSpecifiedFields()
        //{
        //    MappingExtensions.Maps.CreateMap<Category, CategoryDto>();

        //    int existingCategoryId = 5;
        //    var existingCategory = new Category() { Id = existingCategoryId, Name = "some category name" };
        //    string fields = "id,name";

        //    // Arange
        //    var autoMocker = new RhinoAutoMocker<CategoriesController>();
        //    autoMocker.Get<ICategoryApiService>().Stub(x => x.GetCategoryById(existingCategoryId)).Return(existingCategory);
        //    autoMocker.Get<IAclService>().Stub(x => x.GetAclRecords(new Category())).IgnoreArguments().Return(new List<AclRecord>());
        //    autoMocker.Get<IStoreMappingService>().Stub(x => x.GetStoreMappings(new Category())).IgnoreArguments().Return(new List<StoreMapping>());

        //    // Act
        //    autoMocker.ClassUnderTest.GetCategoryById(existingCategoryId, fields);

        //    // Assert
        //    autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
        //        x => x.Serialize(
        //            Arg<CategoriesRootObject>.Matches(objectToSerialize => objectToSerialize.Categories[0].Id == existingCategory.Id.ToString() &&
        //                                                                   objectToSerialize.Categories[0].Name == existingCategory.Name),
        //        Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        //}
    }
}