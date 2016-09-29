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
    public class CategoryApiServiceTests_GetCategoriesCount_ProductIdParameter
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

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo);
        }

        [Test]
        public void WhenCalledWithValidProductId_ShouldReturnOnlyTheCountOfTheCategoriesMappedToThisProduct()
        {
            // Arange
            int productId = 3;
            var expectedCollectionCount = (from cat in _existigCategories
                                          join mapping in _existingCategoryMappings on cat.Id equals mapping.CategoryId
                                          where mapping.ProductId == productId
                                          orderby cat.Id
                                          select cat).Count();

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(productId: productId);

            // Assert
            Assert.AreEqual(expectedCollectionCount, categoriesCount);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledWithNegativeOrZeroProductId_ShouldReturnZero(int productId)
        {
            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(productId: productId);

            // Assert
            Assert.AreEqual(0, categoriesCount);
        }

        [Test]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(5465464)]
        public void WhenCalledWithProductIdThatDoesNotExistInTheMappings_ShouldReturnZero(int productId)
        {
            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(productId: productId);

            // Assert
            Assert.AreEqual(0, categoriesCount);
        }
    }
}