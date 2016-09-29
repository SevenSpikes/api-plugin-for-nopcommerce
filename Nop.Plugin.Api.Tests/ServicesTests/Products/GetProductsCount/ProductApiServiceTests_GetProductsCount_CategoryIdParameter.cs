using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Products.GetProductsCount
{
    [TestFixture]
    public class ProductApiServiceTests_GetProductsCount_CategoryIdParameter
    {
        private IProductApiService _productApiService;
        private List<Product> _existigProducts;
        private List<ProductCategory> _existingCategoryMappings;

        [SetUp]
        public void Setup()
        {
            _existigProducts = new List<Product>()
            {
                new Product() {Id = 2 },
                new Product() {Id = 3 },
                new Product() {Id = 1 },
                new Product() {Id = 4 },
                new Product() {Id = 5 },
                new Product() {Id = 6, Deleted = true },
                new Product() {Id = 7 }
            };

            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigProducts.AsQueryable());

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

            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
        }

        [Test]
        public void WhenCalledWithValidCategoryId_ShouldReturnOnlyTheCountOfTheProductsMappedToThisCategory()
        {
            // Arange
            int categoryId = 3;
            var expectedCollectionCount = (from product in _existigProducts
                                          join mapping in _existingCategoryMappings on product.Id equals mapping.ProductId
                                          where mapping.CategoryId == categoryId
                                          select product).Count();

            // Act
            var productsCount = _productApiService.GetProductsCount(categoryId: categoryId);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledWithNegativeOrZeroCategoryId_ShouldReturnZero(int categoryId)
        {
            // Act
            var productsCount = _productApiService.GetProductsCount(categoryId: categoryId);

            // Assert
            Assert.AreEqual(0, productsCount);
        }

        [Test]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(5465464)]
        public void WhenCalledWithCategoryIdThatDoesNotExistInTheMappings_ShouldReturnZero(int categoryId)
        {
            // Act
            var productsCount = _productApiService.GetProductsCount(categoryId: categoryId);

            // Assert
            Assert.AreEqual(0, productsCount);
        }
    }
}