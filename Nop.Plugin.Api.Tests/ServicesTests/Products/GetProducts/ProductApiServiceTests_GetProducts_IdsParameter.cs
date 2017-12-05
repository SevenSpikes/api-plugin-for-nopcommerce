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
    public class ProductApiServiceTests_GetProducts_IdsParameter
    {
        private IProductApiService _productApiService;
        private List<Product> _existigProducts;

        [SetUp]
        public void Setup()
        {
            _existigProducts = new List<Product>()
            {
                new Product() {Id = 2},
                new Product() {Id = 3},
                new Product() {Id = 1},
                new Product() {Id = 4},
                new Product() {Id = 5},
                new Product() {Id = 6, Deleted = true},
                new Product() {Id = 7, Published = false}
            };

            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();
            
            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
        }
        
        [Test]
        public void WhenCalledWithIdsParameter_GivenProductsWithTheSpecifiedIds_ShouldReturnThemSortedById()
        {
            var idsCollection = new List<int>() { 1, 5 };

            var products = _productApiService.GetProducts(ids: idsCollection);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(idsCollection[0], products[0].Id);
            Assert.AreEqual(idsCollection[1], products[1].Id);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenProductsWithSomeOfTheSpecifiedIds_ShouldReturnThemSortedById()
        {
            var idsCollection = new List<int>() { 1, 5, 97373, 4 };

            var products = _productApiService.GetProducts(ids: idsCollection);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(idsCollection[0], products[0].Id);
            Assert.AreEqual(idsCollection[3], products[1].Id);
            Assert.AreEqual(idsCollection[1], products[2].Id);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenProductsThatDoNotMatchTheSpecifiedIds_ShouldReturnEmptyCollection()
        {
            var idsCollection = new List<int>() { 2123434, 5456456, 97373, -45 };

            var products = _productApiService.GetProducts(ids: idsCollection);

            // Assert
            CollectionAssert.IsEmpty(products);
        }

        [Test]
        public void WhenCalledWithIdsParameter_GivenEmptyIdsCollection_ShouldReturnAllNotDeletedCategories()
        {
            var idsCollection = new List<int>();

            var products = _productApiService.GetProducts(ids: idsCollection);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(products.Count, _existigProducts.Count(x => !x.Deleted));
        }
    }
}