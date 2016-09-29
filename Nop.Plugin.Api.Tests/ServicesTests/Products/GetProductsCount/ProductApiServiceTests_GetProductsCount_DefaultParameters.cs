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
    public class ProductApiServiceTests_GetProductsCount_DefaultParameters
    {
        [Test]
        public void WhenCalledWithDefaultParameters_GivenNoProductsExist_ShouldReturnZero()
        {
            // Arange
            var productsRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productsRepo.Stub(x => x.TableNoTracking).Return(new List<Product>().AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            // Act
            var cut = new ProductApiService(productsRepo, productCategoryRepo, vendorRepo);
            var productsCount = cut.GetProductsCount();

            // Assert
            Assert.AreEqual(0, productsCount);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenOnlyDeletedProductsExist_ShouldReturnZero()
        {
            var existingProducts = new List<Product>();
            existingProducts.Add(new Product() { Id = 1, Deleted = true });
            existingProducts.Add(new Product() { Id = 2, Deleted = true });

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(existingProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
            var countResult = cut.GetProductsCount();

            // Assert
            Assert.AreEqual(0, countResult);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeNotDeletedProductsExist_ShouldReturnTheirCount()
        {
            var existingProducts = new List<Product>();
            existingProducts.Add(new Product() { Id = 1 });
            existingProducts.Add(new Product() { Id = 2, Deleted = true });
            existingProducts.Add(new Product() { Id = 3 });

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(existingProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
            var countResult = cut.GetProductsCount();

            // Assert
            Assert.AreEqual(2, countResult);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeProductsExist_ShouldReturnTheirCount()
        {
            var existingProducts = new List<Product>();
            existingProducts.Add(new Product() { Id = 2, Published = false });
            existingProducts.Add(new Product() { Id = 3 });
            existingProducts.Add(new Product() { Id = 1 });

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(existingProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
            var countResult = cut.GetProductsCount();

            // Assert
            Assert.AreEqual(existingProducts.Count, countResult);
        }
    }
}