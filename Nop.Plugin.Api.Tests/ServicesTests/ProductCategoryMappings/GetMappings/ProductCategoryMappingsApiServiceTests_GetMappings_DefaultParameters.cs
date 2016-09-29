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
    public class ProductCategoryMappingsApiServiceTests_GetMappings_DefaultParameters
    {
        [Test]
        public void GivenNonEmptyValidRepositoryWithMoreThanTheMaxItems_ShouldReturnDefaultLimitItems()
        {
            var repo = new List<ProductCategory>();

            var randomNumber = new Random();

            var currentRepoSize = Configurations.MaxLimit * 2;

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

            var result = cut.GetMappings();

            // Assert
            Assert.IsNotEmpty(result);
            Assert.AreEqual(Configurations.DefaultLimit, result.Count);
        }

        [Test]
        public void GivenEmptyRepository_ShouldReturnEmptyCollection()
        {
            var repo = new List<ProductCategory>();
           
            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            var result = cut.GetMappings();

            // Assert
            Assert.IsEmpty(result);
        }
    }
}