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
    public class CategoriesApiServiceTests_GetCategories_ProductIdParameter
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;
        private List<ProductCategory> _existingCategoryMappings;

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

            _existingCategoryMappings= new List<ProductCategory>()
            {
                new ProductCategory() { CategoryId = 1, ProductId = 2 }, 
                new ProductCategory() { CategoryId = 1, ProductId = 3 },
                new ProductCategory() { CategoryId = 2, ProductId = 2 },
                new ProductCategory() { CategoryId = 3, ProductId = 1 },
                new ProductCategory() { CategoryId = 4, ProductId = 4 },
                new ProductCategory() { CategoryId = 5, ProductId = 5 }
            };

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            productCategoryRepo.Stub(x => x.TableNoTracking).Return(_existingCategoryMappings.AsQueryable());

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();
            storeMappingService.Stub(x => x.Authorize(Arg<Category>.Is.Anything)).Return(true);

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
        }

        [Test]
        public void WhenCalledWithValidProductId_ShouldReturnOnlyTheCategoriesMappedToThisProduct()
        {
            // Arange
            int productId = 3;
            var expectedCollection = (from cat in _existigCategories
                                      join mapping in _existingCategoryMappings on cat.Id equals mapping.CategoryId
                                      where mapping.ProductId == productId
                                      orderby cat.Id
                                      select cat);

            // Act
            var categories = _categoryApiService.GetCategories(productId: productId);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledWithNegativeOrZeroProductId_ShouldReturnEmptyCollection(int productId)
        {
            // Act
            var categories = _categoryApiService.GetCategories(productId: productId);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }

        [Test]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(5465464)]
        public void WhenCalledWithProductIdThatDoesNotExistInTheMappings_ShouldReturnEmptyCollection(int productId)
        {
            // Arange
            // Act
            var categories = _categoryApiService.GetCategories(productId: productId);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }
    }
}