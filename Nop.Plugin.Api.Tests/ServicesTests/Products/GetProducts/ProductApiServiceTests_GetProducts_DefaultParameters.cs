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
    public class ProductApiServiceTests_GetProducts_DefaultParameters
    {
        [Test]
        public void WhenCalledWithDefaultParameters_GivenNoProductsExist_ShouldReturnEmptyCollection()
        {
            // Arange
            var productsRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productsRepo.Stub(x => x.TableNoTracking).Return(new List<Product>().AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new ProductApiService(productsRepo, productCategoryRepo, vendorRepo, storeMappingService);
            var products = cut.GetProducts();

            // Assert
            CollectionAssert.IsEmpty(products);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenOnlyDeletedProductsExist_ShouldReturnEmptyCollection()
        {
            var existingProducts = new List<Product>();
            existingProducts.Add(new Product() { Id = 1, Deleted = true });
            existingProducts.Add(new Product() { Id = 2, Deleted = true });

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(existingProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
            var products = cut.GetProducts();

            // Assert
            CollectionAssert.IsEmpty(products);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeNotDeletedProductsExist_ShouldReturnThemSortedById()
        {
            var existingProducts = new List<Product>();
            existingProducts.Add(new Product() { Id = 1 });
            existingProducts.Add(new Product() { Id = 2, Deleted = true });
            existingProducts.Add(new Product() { Id = 3 });

            var expectedCollection = existingProducts.Where(x => !x.Deleted).OrderBy(x => x.Id);

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(existingProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
            var products = cut.GetProducts();

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedCollection.Count(), products.Count);
            Assert.IsTrue(products.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeProductsExist_ShouldReturnThemSortedById()
        {
            var existingProducts = new List<Product>();
            existingProducts.Add(new Product() { Id = 2, Published = false });
            existingProducts.Add(new Product() { Id = 3 });
            existingProducts.Add(new Product() { Id = 1 });

            var expectedCollection = existingProducts.Where(x => !x.Deleted).OrderBy(x => x.Id);

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.TableNoTracking).Return(existingProducts.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
            var products = cut.GetProducts();

            // Assert
            CollectionAssert.IsNotEmpty(products);
            Assert.AreEqual(expectedCollection.Count(), products.Count);
            Assert.IsTrue(products.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }
    }
}