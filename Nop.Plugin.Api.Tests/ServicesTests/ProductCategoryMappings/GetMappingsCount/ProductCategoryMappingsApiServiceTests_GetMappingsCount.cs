using System;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Api.Tests.ServicesTests.ProductCategoryMappings.GetMappingsCount
{
    [TestFixture]
    public class ProductCategoryMappingsApiServiceTests_GetMappingsCount
    {
        private List<ProductCategory> _repo;

        [SetUp]
        public void Setup()
        {
            _repo = new List<ProductCategory>();

            var randomNumber = new Random();

            var currentRepoSize = randomNumber.Next(10, 100);

            _repo.Add(new ProductCategory()
            {
                CategoryId = 1,
                ProductId = 2,
            });

            _repo.Add(new ProductCategory()
            {
                CategoryId = 2,
                ProductId = 1,
            });

            for (int i = 0; i < currentRepoSize; i++)
            {
                _repo.Add(new ProductCategory()
                {
                    CategoryId = 1,
                    ProductId = randomNumber.Next(1, 2),
                });

                _repo.Add(new ProductCategory()
                {
                    CategoryId = 2,
                    ProductId = randomNumber.Next(1, 2),
                });
            }
        }

        [Test]
        public void GivenNonEmptyValidRepository_WhenCalledWithDefaultParameters_ShouldReturnRepositorySize()
        {
            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(_repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            int result = cut.GetMappingsCount();

            // Assert
            Assert.AreEqual(_repo.Count, result);
        }

        [Test]
        public void GivenEmptyRepository_WhenCalledWithDefaultParameters_ShouldReturnZero()
        {
            var repo = new List<ProductCategory>();

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            int result = cut.GetMappingsCount();

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [TestCase(1, 2)]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        public void GivenNonEmptyValidRepository_WhenCalledWithSomeParameters_ShouldReturnCountOfAllItemsAccordingToParameters(int categoryId, int productId)
        {
            var expectedCount = _repo.Count(x => x.CategoryId == categoryId && x.ProductId == productId);

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(_repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            int result = cut.GetMappingsCount(productId, categoryId);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenNonEmptyValidRepository_WhenCalledWithCategoryIdParameter_ShouldReturnCountOfAllItemsThatAreMappedToThisCategory(int categoryId)
        {
            var expectedCount = _repo.Count(x => x.CategoryId == categoryId);

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(_repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            int result = cut.GetMappingsCount(categoryId: categoryId);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenNonEmptyValidRepository_WhenCalledWithProductIdParameter_ShouldReturnCountOfAllItemsThatAreMappedToThisProduct(int productId)
        {
            var expectedCount = _repo.Count(x => x.ProductId == productId);

            // Arange
            var mappingRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            mappingRepo.Stub(x => x.TableNoTracking).Return(_repo.AsQueryable());

            // Act
            var cut = new ProductCategoryMappingsApiService(mappingRepo);

            int result = cut.GetMappingsCount(productId: productId);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }
    }
}