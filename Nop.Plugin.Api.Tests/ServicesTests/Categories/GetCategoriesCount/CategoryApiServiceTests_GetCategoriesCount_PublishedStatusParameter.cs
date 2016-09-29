using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories.GetCategoriesCount
{
    [TestFixture]
    public class CategoryApiServiceTests_GetCategoriesCount_PublishedStatusParameter
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;

        [SetUp]
        public void Setup()
        {
            _existigCategories = new List<Category>()
            {
                new Category() {Id = 2, Published = true },
                new Category() {Id = 3, Published = true  },
                new Category() {Id = 1, Published = false  },
                new Category() {Id = 4, Published = true  },
                new Category() {Id = 5, Published = true  },
                new Category() {Id = 6, Deleted = true, Published = true  },
                new Category() {Id = 7, Published = false }
            };

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo);
        }

        [Test]
        public void WhenAskForOnlyThePublishedCategories_ShouldReturnOnlyThePublishedCategoriesCount()
        {
            // Arange
            var expectedCategoriesCount = _existigCategories.Count(x => x.Published && !x.Deleted);

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(publishedStatus: true);

            // Assert
            Assert.AreEqual(expectedCategoriesCount, categoriesCount);
        }

        [Test]
        public void WhenAskForOnlyTheUnpublishedCategories_ShouldReturnOnlyTheUnpublishedCategoriesCount()
        {
            // Arange
            var expectedCollectionCount = _existigCategories.Count(x => !x.Published && !x.Deleted);

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(publishedStatus: false);

            // Assert
            Assert.AreEqual(expectedCollectionCount, categoriesCount);
        }
    }
}