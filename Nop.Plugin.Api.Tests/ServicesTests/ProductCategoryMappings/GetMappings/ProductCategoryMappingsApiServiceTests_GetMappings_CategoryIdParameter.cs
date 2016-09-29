using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.ProductCategoryMappings.GetMappings
{
    [TestFixture]
    public class ProductCategoryMappingsApiServiceTests_GetMappings_CategoryIdParameter
    {
        [Test]
        [TestCase(0)]
        [TestCase(-132340)]
        public void GivenNegativeOrZeroCategoryId_ShouldReturnEmptyCollection(int categoryId)
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

            var result = cut.GetMappings(categoryId: categoryId);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GivenPositiveCategoryId_ShouldReturnCollectionContainingAllMappingsWithThisCategoryId()
        {
            var repo = new List<ProductCategory>();

            var randomNumber = new Random();

            var currentRepoSize = randomNumber.Next(51, 100);
            
            for (int i = 0; i < currentRepoSize; i++)
            {
                repo.Add(new ProductCategory()
                {
                    CategoryId = randomNumber.Next(1, 2),
                    ProductId = randomNumber.Next(10, 20),
                });
            }

            var categoryId = 1;

            repo.Add(new ProductCategory()
            {
                CategoryId = categoryId,
                ProductId = randomNumber.Next(10, 20),
            });

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            var result = cut.GetMappings(categoryId: categoryId);

            // Assert
            Assert.IsTrue(result.Select(x => new { x.CategoryId, x.ProductId })
                                .SequenceEqual(repo.Where(x => x.CategoryId == categoryId).Take(Configurations.DefaultLimit).Select(x => new { x.CategoryId, x.ProductId })));
        }
    }
}