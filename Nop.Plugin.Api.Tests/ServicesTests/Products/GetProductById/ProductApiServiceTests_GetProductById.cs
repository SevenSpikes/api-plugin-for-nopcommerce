using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Products.GetProductById
{
    using Nop.Services.Stores;

    [TestFixture]
    public class ProductApiServiceTests_GetProductById
    {
        [Test]
        public void WhenNullIsReturnedByTheRepository_ShouldReturnNull()
        {
            int productId = 3;

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.GetById(productId)).Return(null);
            productRepo.Stub(x => x.Table).Return((new List<Product>()).AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
            var result = cut.GetProductById(productId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(-2)]
        [TestCase(0)]
        public void WhenNegativeOrZeroProductIdPassed_ShouldReturnNull(int negativeOrZeroProductId)
        {
            // Aranges
            var productRepoStub = MockRepository.GenerateStub<IRepository<Product>>();
            productRepoStub.Stub(x => x.Table).Return((new List<Product>()).AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new ProductApiService(productRepoStub, productCategoryRepo, vendorRepo, storeMappingService);
            var result = cut.GetProductById(negativeOrZeroProductId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void WhenProductIsReturnedByTheRepository_ShouldReturnTheSameProduct()
        {
            int productId = 3;
            var product = new Product() { Id = 3, Name = "some name" };

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.GetById(productId)).Return(product);

            var list = new List<Product>();
            list.Add(product);

            productRepo.Stub(x => x.Table).Return(list.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo, storeMappingService);
            var result = cut.GetProductById(productId);

            // Assert
            Assert.AreSame(product, result);
        }
    }
}