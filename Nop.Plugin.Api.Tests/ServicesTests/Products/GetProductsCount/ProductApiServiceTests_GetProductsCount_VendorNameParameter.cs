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
    using Nop.Services.Stores;

    [TestFixture]
    public class ProductApiServiceTests_GetProductsCount_VendorNameParameter
    {
        private IProductApiService _productApiService;
        private List<Product> _existigProducts;
        private List<Vendor> _existingVendors;

        [SetUp]
        public void Setup()
        {
            _existigProducts = new List<Product>()
            {
                new Product() {Id = 2, VendorId = 1},
                new Product() {Id = 3, VendorId = 2 },
                new Product() {Id = 1, VendorId = 1 },
                new Product() {Id = 4 },
                new Product() {Id = 5 },
                new Product() {Id = 6, Deleted = true, VendorId = 1 },
                new Product() {Id = 7, VendorId = 2 },
                new Product() {Id = 8, VendorId = 3 },
                new Product() {Id = 9, VendorId = 4 }
            };

            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(_existigProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            _existingVendors = new List<Vendor>()
            {
                new Vendor() {Id = 1, Name = "vendor 1", Active = true},
                new Vendor() {Id = 2, Name = "vendor 2", Active = true},
                new Vendor() {Id = 3, Name = "vendor 3", Deleted = true},
                new Vendor() {Id = 4, Name = "vendor 4", Active = false},
                new Vendor() {Id = 5, Name = "vendor 5", Active = true}
            };

            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();
            vendorRepo.Stub(x => x.TableNoTracking).Return(_existingVendors.AsQueryable());

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();
            storeMappingService.Stub(x => x.Authorize(Arg<Product>.Is.Anything)).Return(true);

            _productApiService = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
        }

        [Test]
        [TestCase("vendor 1")]
        [TestCase("vendor 2")]
        public void WhenCalledWithValidVendorName_ShouldReturnOnlyTheCountOfTheProductsMappedToThisVendor(string vendorName)
        {
            // Arange
            var expectedCollectionCount = (from product in _existigProducts
                                          join vendor in _existingVendors on product.VendorId equals vendor.Id
                                          where vendor.Name == vendorName && !vendor.Deleted && vendor.Active
                                          select product).Count();

            // Act
            var productsCount = _productApiService.GetProductsCount(vendorName: vendorName);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }

        [Test]
        public void WhenCalledWithValidVendorName_GivenVendorCollectionWhereThisVendorIsDeleted_ShouldReturnZero()
        {
            // Arange
            string vendorName = "vendor 3";
            var expectedCollectionCount = (from product in _existigProducts
                                           join vendor in _existingVendors on product.VendorId equals vendor.Id
                                           where vendor.Name == vendorName && !vendor.Deleted && vendor.Active
                                           select product).Count();

            // Act
            var productsCount = _productApiService.GetProductsCount(vendorName: vendorName);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }

        [Test]
        public void WhenCalledWithValidVendorName_GivenVendorCollectionWhereThisVendorIsUnactive_ShouldReturnZero()
        {
            // Arange
            string vendorName = "vendor 4";
            var expectedCollectionCount = (from product in _existigProducts
                                           join vendor in _existingVendors on product.VendorId equals vendor.Id
                                           where vendor.Name == vendorName && !vendor.Deleted && vendor.Active
                                           select product).Count();

            // Act
            var productsCount = _productApiService.GetProductsCount(vendorName: vendorName);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }

        [Test]
        public void WhenCalledWithValidVendorName_GivenProductsCollectionWhereThereAreNoProductsMappedToThisVendor_ShouldReturnZero()
        {
            // Arange
            string vendorName = "vendor 5";
            var expectedCollectionCount = (from product in _existigProducts
                                           join vendor in _existingVendors on product.VendorId equals vendor.Id
                                           where vendor.Name == vendorName && !vendor.Deleted && vendor.Active
                                           select product).Count();

            // Act
            var productsCount = _productApiService.GetProductsCount(vendorName: vendorName);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void WhenCalledWithNullOrEmptyVendorName_ShouldReturnTheEntireCollectionCount(string vendorName)
        {
            // Arange
            var expectedCollectionCount = _existigProducts.Count(p => !p.Deleted);

            // Act
            var productsCount = _productApiService.GetProductsCount(vendorName: vendorName);

            // Assert
            Assert.AreEqual(expectedCollectionCount, productsCount);
        }

        [Test]
        [TestCase("invalid vendor name")]
        [TestCase("$&@#*@(!(&%)@_@_!*%^&/sd")]
        [TestCase("345")]
        public void WhenCalledWithInvalidVendorName_ShouldReturnZero(string vendorName)
        {
            // Act
            var productsCount = _productApiService.GetProductsCount(vendorName: vendorName);

            // Assert
            Assert.AreEqual(0, productsCount);
        }
    }
}