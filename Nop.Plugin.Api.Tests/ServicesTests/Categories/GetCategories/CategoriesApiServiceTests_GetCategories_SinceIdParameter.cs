using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories.GetCategories
{
    using Nop.Services.Stores;

    [TestFixture]
    public class CategoriesApiServiceTests_GetCategories_SinceIdParameter
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;

        [SetUp]
        public void Setup()
        {
            _existigCategories = new List<Category>()
            {
                new Category() {Id = 2 },
                new Category() {Id = 3 },
                new Category() {Id = 1 },
                new Category() {Id = 4 },
                new Category() {Id = 5 },
                new Category() {Id = 6, Deleted = true },
                new Category() {Id = 7 }
            };

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
        }

        [Test]
        public void WhenCalledWithValidSinceId_ShouldReturnOnlyTheCategoriesAfterThisIdSortedById()
        {
            // Arange
            int sinceId = 3;
            var expectedCollection = _existigCategories.Where(x => x.Id > sinceId && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var categories = _categoryApiService.GetCategories(sinceId: sinceId);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledZeroOrNegativeSinceId_ShouldReturnAllTheCategoriesSortedById(int sinceId)
        {
            // Arange
            var expectedCollection = _existigCategories.Where(x => x.Id > sinceId && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var categories = _categoryApiService.GetCategories(sinceId: sinceId);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledSinceIdOutsideOfTheCategoriesIdsRange_ShouldReturnEmptyCollection()
        {
            // Arange
            int sinceId = int.MaxValue;
        
            // Act
            var categories = _categoryApiService.GetCategories(sinceId: sinceId);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }
    }
}