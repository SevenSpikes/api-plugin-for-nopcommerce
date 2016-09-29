using System;
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
    public class ProductApiServiceTests_GetProductsCount_CreatedParameters
    {
        private IProductApiService _productApiService;
        private List<Product> _existigProducts;
        private DateTime _baseDate;

        [SetUp]
        public void Setup()
        {
            _baseDate = new DateTime(2016, 2, 23);

            _existigProducts = new List<Product>()
            {
                new Product() {Id = 2, CreatedOnUtc = _baseDate.AddMonths(2) },
                new Product() {Id = 3, CreatedOnUtc = _baseDate.AddMonths(10) },
                new Product() {Id = 1, CreatedOnUtc = _baseDate.AddMonths(7) },
                new Product() {Id = 4, CreatedOnUtc = _baseDate },
                new Product() {Id = 5, CreatedOnUtc = _baseDate.AddMonths(3) },
                new Product() {Id = 6, Deleted = true, CreatedOnUtc = _baseDate.AddMonths(10) },
                new Product() {Id = 7, Published = false, CreatedOnUtc = _baseDate.AddMonths(4) }
            };

            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
        }

        [Test]
        public void WhenCalledWithCreatedAtMinParameter_GivenSomeProductsCreatedAfterThatDate_ShouldReturnTheirCount()
        {
            // Arange
            DateTime createdAtMinDate = _baseDate.AddMonths(5);
            var expectedCollection =
                _existigProducts.Where(x => x.CreatedOnUtc > createdAtMinDate && !x.Deleted);
            var expectedProductsCount = expectedCollection.Count();

            // Act
            var productsCount = _productApiService.GetProductsCount(createdAtMin: createdAtMinDate);

            // Assert
            Assert.AreEqual(expectedProductsCount, productsCount);
        }

        [Test]
        public void WhenCalledWithCreatedAtMinParameter_GivenAllCategoriesCreatedBeforeThatDate_ShouldReturnZero()
        {
            // Arange
            DateTime createdAtMinDate = _baseDate.AddMonths(11);

            // Act
            var productsCount = _productApiService.GetProductsCount(createdAtMin: createdAtMinDate);

            // Assert
            Assert.AreEqual(0, productsCount);
        }

        [Test]
        public void WhenCalledWithCreatedAtMaxParameter_GivenSomeProductsCreatedBeforeThatDate_ShouldReturnTheirCount()
        {
            // Arange
            DateTime createdAtMaxDate = _baseDate.AddMonths(5);
            var expectedCollection = _existigProducts.Where(x => x.CreatedOnUtc < createdAtMaxDate && !x.Deleted);
            var expectedProductsCount = expectedCollection.Count();

            // Act
            var productsCount = _productApiService.GetProductsCount(createdAtMax: createdAtMaxDate);

            // Assert
            Assert.AreEqual(expectedProductsCount, productsCount);
        }

        [Test]
        public void WhenCalledWithCreatedAtMaxParameter_GivenAllProductsCreatedAfterThatDate_ShouldReturnZero()
        {
            // Arange
            DateTime createdAtMaxDate = _baseDate.Subtract(new TimeSpan(365)); // subtract one year

            // Act
            var productsCount = _productApiService.GetProductsCount(createdAtMax: createdAtMaxDate);

            // Assert
            Assert.AreEqual(0, productsCount);
        }
    }
}