using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.ProductCategoryMappings.GetMappings
{
    [TestFixture]
    public class ProductCategoryMappingsApiServiceTests_GetMappings_ProductIdParameter
    {
        [Test]
        [TestCase(0)]
        [TestCase(-132340)]
        public void GivenNegativeOrZeroProductId_ShouldReturnEmptyCollection(int productId)
        {
            var repo = new List<ProductCategory>();

            var randomNumber = new Random();

            var currentRepoSize = randomNumber.Next(10, 100);

            for (int i = 0; i < currentRepoSize; i++)
            {
                repo.Add(new ProductCategory()
                {
                    CategoryId = randomNumber.Next(10, 20),
                    ProductId = randomNumber.Next(1, 2),
                });
            }

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            var result = cut.GetMappings(productId: productId);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GivenPositiveProductId_ShouldReturnCollectionContainingAllMappingsWithThisProductId()
        {
            var repo = new List<ProductCategory>();

            var randomNumber = new Random();

            var productId = 1;

            repo.Add(new ProductCategory()
            {
                CategoryId = randomNumber.Next(10, 20),
                ProductId = productId,
            });

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            var result = cut.GetMappings(productId: productId);

            // Assert
            Assert.IsTrue(result.Select(x => new { x.CategoryId, x.ProductId })
                                .SequenceEqual(repo.Where(x => x.ProductId == productId).Select(x => new { x.CategoryId, x.ProductId })));
        }
    }
}