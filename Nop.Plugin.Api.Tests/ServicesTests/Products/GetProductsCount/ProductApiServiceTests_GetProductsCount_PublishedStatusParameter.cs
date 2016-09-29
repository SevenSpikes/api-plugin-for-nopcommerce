using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories.GetCategoriesCount
{
    [TestFixture]
    public class ProductApiServiceTests_GetProductsCount_PublishedStatusParameter
    {
        private IProductApiService _productApiService;
        private List<Product> _existigProducts;

        [SetUp]
        public void Setup()
        {
            _existigProducts = new List<Product>()
            {
                new Product() {Id = 2, Published = true },
                new Product() {Id = 3, Published = true  },
                new Product() {Id = 1, Published = false  },
                new Product() {Id = 4, Published = true  },
                new Product() {Id = 5, Published = true  },
                new Product() {Id = 6, Deleted = true, Published = true  },
                new Product() {Id = 7, Published = false }
            };

            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
        }

        [Test]
        public void WhenAskForOnlyThePublishedProducts_ShouldReturnOnlyThePublishedProductsCount()
        {
            // Arange
            var expectedProductsCount = _existigProducts.Count(x => x.Published && !x.Deleted);

            // Act
            var productsCount = _productApiService.GetProductsCount(publishedStatus: true);

            // Assert
            Assert.AreEqual(expectedProductsCount, productsCount);
        }

        [Test]
        public void WhenAskForOnlyTheUnpublishedProducts_ShouldReturnOnlyTheUnpublishedProductsCount()
        {
            // Arange
            var expectedCollectionCount = _existigProducts.Count(x => !x.Published && !x.Deleted);

            // Act
            var productsCount = _productApiService.GetProductsCount(publishedStatus: false);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }
    }
}