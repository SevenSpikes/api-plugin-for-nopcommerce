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
    public class CategoryApiServiceTests_GetCategories_IdsParameter
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;

        [SetUp]
        public void Setup()
        {
            _existigCategories = new List<Category>()
            {
                new Category() {Id = 2},
                new Category() {Id = 3},
                new Category() {Id = 1},
                new Category() {Id = 4},
                new Category() {Id = 5},
                new Category() {Id = 6, Deleted = true},
                new Category() {Id = 7, Published = false}
            };

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo);
        }
        
        [Test]
        public void WhenCalledWithIdsParameter_GivenCategoriesWithTheSpecifiedIds_ShouldReturnThemSortedById()
        {
            var idsCollection = new List<int>() { 1, 5 };

            var categories = _categoryApiService.GetCategories(ids: idsCollection);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(idsCollection[0], categories[0].Id);
            Assert.AreEqual(idsCollection[1], categories[1].Id);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenCategoriesWithSomeOfTheSpecifiedIds_ShouldReturnThemSortedById()
        {
            var idsCollection = new List<int>() { 1, 5, 97373, 4 };

            var categories = _categoryApiService.GetCategories(ids: idsCollection);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(idsCollection[0], categories[0].Id);
            Assert.AreEqual(idsCollection[3], categories[1].Id);
            Assert.AreEqual(idsCollection[1], categories[2].Id);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenCategoriesThatDoNotMatchTheSpecifiedIds_ShouldReturnEmptyCollection()
        {
            var idsCollection = new List<int>() { 2123434, 5456456, 97373, -45 };

            var categories = _categoryApiService.GetCategories(ids: idsCollection);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenEmptyIdsCollection_ShouldReturnAllNotDeletedCategories()
        {
            var idsCollection = new List<int>();

            var categories = _categoryApiService.GetCategories(ids: idsCollection);

            // Assert
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(categories.Count, _existigCategories.Count(x => !x.Deleted));
        }
    }
}