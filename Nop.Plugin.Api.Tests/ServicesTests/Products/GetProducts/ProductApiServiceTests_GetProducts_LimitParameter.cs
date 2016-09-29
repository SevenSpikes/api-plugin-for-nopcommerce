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
    [TestFixture]
    public class ProductApiServiceTests_GetProducts_LimitParameter
    {
        private IProductApiService _productApiService;
        private List<Product> _existigProducts;

        [SetUp]
        public void Setup()
        {
            _existigProducts = new List<Product>();

            for (int i = 0; i < 1000; i++)
            {
                _existigProducts.Add(new Product()
                {
                    Id = i + 1
                });
            }

            _existigProducts[5].Deleted = true;
            _existigProducts[51].Published = false;

            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
        }

        [Test]
        public void WhenCalledWithLimitParameter_GivenProductsAboveTheLimit_ShouldReturnCollectionWithCountEqualToTheLimit()
        {
            //Arange
            var expectedLimit = 5;

            //Act
            var products = _productApiService.GetProducts(limit: expectedLimit);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedLimit, products.Count);
        }

        [Test]
        public void WhenCalledWithLimitParameter_GivenProductsBellowTheLimit_ShouldReturnCollectionWithCountEqualToTheAvailableProducts()
        {
            //Arange
            var expectedLimit = _existigProducts.Count(x => !x.Deleted);

            //Act
            var products = _productApiService.GetProducts(limit: expectedLimit + 10);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedLimit, products.Count);
        }

        [Test]
        public void WhenCalledWithZeroLimitParameter_GivenSomeProducts_ShouldReturnEmptyCollection()
        {
            //Arange
            var expectedLimit = 0;

            //Act
            var products = _productApiService.GetProducts(limit: expectedLimit);

            // Assert
            CollectionAssert.IsEmpty(products);
        }

        [Test]
        public void WhenCalledWithNegativeLimitParameter_GivenSomeProducts_ShouldReturnEmptyCollection()
        {
            //Arange
            var expectedLimit = -10;

            //Act
            var products = _productApiService.GetProducts(limit: expectedLimit);

            // Assert
            CollectionAssert.IsEmpty(products);
        }
    }
}