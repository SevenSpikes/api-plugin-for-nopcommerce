using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Products.GetProducts
{
    using Nop.Services.Stores;

    [TestFixture]
    public class ProductApiServiceTests_GetProducts_PublishedStatusParameter
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

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
        }

        [Test]
        public void WhenAskForOnlyThePublishedProducts_ShouldReturnOnlyThePublishedProductsOrderedByid()
        {
            // Arange
            var expectedProducts = _existigProducts.Where(x => x.Published && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var products = _productApiService.GetProducts(publishedStatus: true);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedProducts.Count(), products.Count);
            Assert.IsTrue(products.Select(x => x.Id).SequenceEqual(expectedProducts.Select(x => x.Id)));
        }

        [Test]
        public void WhenAskForOnlyTheUnpublishedProducts_ShouldReturnOnlyTheUnpublishedProductsCount()
        {
            // Arange
            var expectedCollection = _existigProducts.Where(x => !x.Published && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var products = _productApiService.GetProducts(publishedStatus: false);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedCollection.Count(), products.Count);
            Assert.IsTrue(products.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }
    }
}