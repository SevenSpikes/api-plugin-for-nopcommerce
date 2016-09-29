using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Products.GetProducts
{
    [TestFixture]
    public class ProductApiServiceTests_GetProducts_PageParameter
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
            _existigProducts = _existigProducts.OrderBy(x => x.Id).ToList();

            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
        }

        [Test]
        public void WhenCalledWithPageParameter_GivenLimitedProductsCollection_ShouldReturnThePartOfTheCollectionThatCorrespondsToThePage()
        {
            //Arange
            var limit = 5;
            var page = 6;
            var expectedCollection = new ApiList<Product>(_existigProducts.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var products = _productApiService.GetProducts(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedCollection.Count(), products.Count);
            Assert.IsTrue(products.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithZeroPageParameter_GivenLimitedProductsCollection_ShouldReturnTheFirstPage()
        {
            //Arange
            var limit = 5;
            var page = 0;
            var expectedCollection = new ApiList<Product>(_existigProducts.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var products = _productApiService.GetProducts(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedCollection.Count(), products.Count);
            Assert.AreEqual(_existigProducts.First().Id, products.First().Id);
            Assert.IsTrue(products.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithNegativePageParameter_GivenLimitedProductsCollection_ShouldReturnTheFirstPage()
        {
            //Arange
            var limit = 5;
            var page = -30;
            var expectedCollection = new ApiList<Product>(_existigProducts.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var products = _productApiService.GetProducts(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedCollection.Count(), products.Count);
            Assert.AreEqual(_existigProducts.First().Id, products.First().Id);
            Assert.IsTrue(products.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithTooBigPageParameter_GivenLimitedProductsCollection_ShouldReturnEmptyCollection()
        {
            //Arange
            var limit = 5;
            var page = _existigProducts.Count / limit + 100;
            
            //Act
            var products = _productApiService.GetProducts(limit: limit, page: page);

            // Assert
            CollectionAssert.IsEmpty(products);
        }
    }
}