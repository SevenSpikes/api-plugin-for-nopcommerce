using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories.GetCategories
{
    [TestFixture]
    public class CategoryApiServiceTests_GetCategories_LimitParameter
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;

        [SetUp]
        public void Setup()
        {
            _existigCategories = new List<Category>();

            for (int i = 0; i < 1000; i++)
            {
                _existigCategories.Add(new Category()
                {
                    Id = i + 1
                });
            }

            _existigCategories[5].Deleted = true;
            _existigCategories[51].Published = false;

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo);
        }

        [Test]
        public void WhenCalledWithLimitParameter_GivenCategoriesAboveTheLimit_ShouldReturnCollectionWithCountEqualToTheLimit()
        {
            //Arange
            var expectedLimit = 5;

            //Act
            var categories = _categoryApiService.GetCategories(limit: expectedLimit);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedLimit, categories.Count);
        }

        [Test]
        public void WhenCalledWithLimitParameter_GivenCategoriesBellowTheLimit_ShouldReturnCollectionWithCountEqualToTheAvailableCategories()
        {
            //Arange
            var expectedLimit = _existigCategories.Count(x => !x.Deleted);

            //Act
            var categories = _categoryApiService.GetCategories(limit: expectedLimit + 10);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedLimit, categories.Count);
        }

        [Test]
        public void WhenCalledWithZeroLimitParameter_GivenSomeCategories_ShouldReturnEmptyCollection()
        {
            //Arange
            var expectedLimit = 0;

            //Act
            var categories = _categoryApiService.GetCategories(limit: expectedLimit);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }

        [Test]
        public void WhenCalledWithNegativeLimitParameter_GivenSomeCategories_ShouldReturnEmptyCollection()
        {
            //Arange
            var expectedLimit = -10;

            //Act
            var categories = _categoryApiService.GetCategories(limit: expectedLimit);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }
    }
}