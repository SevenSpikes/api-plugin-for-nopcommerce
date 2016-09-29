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
    public class ProductCategoryMappingsApiServiceTests_GetMappings_SinceIdParameter
    {
        private IProductCategoryMappingsApiService _productCategoryMappingsService;
        private List<ProductCategory> _existigMappings;

        // Here the product and category ids does not matter so we can use the setup method.
        [SetUp]
        public void Setup()
        {
            var randomNumber = new Random();

            _existigMappings = new List<ProductCategory>();

            for (int i = 1; i <= 1000; i++)
            {
                _existigMappings.Add(new ProductCategory()
                {
                    Id = i,
                    ProductId = randomNumber.Next(1, 1000),
                    CategoryId = randomNumber.Next(1, 1000),
                });
            }

            _existigMappings = _existigMappings.OrderBy(x => x.Id).ToList();

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            productCategoryRepo.Stub(x => x.TableNoTracking).Return(_existigMappings.AsQueryable());

            _productCategoryMappingsService = new ProductCategoryMappingsApiService(productCategoryRepo);
        }

        [Test]
        public void WhenCalledWithValidSinceId_ShouldReturnOnlyTheMappingsAfterThisIdSortedById()
        {
            // Arange
            var sinceId = 10;
            var expectedCollection =
                _existigMappings.Where(x => x.Id > sinceId).OrderBy(x => x.Id).Take(Configurations.DefaultLimit);

            // Act
            var mappings = _productCategoryMappingsService.GetMappings(sinceId: 10);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(mappings);
            Assert.IsTrue(expectedCollection.Select(x => x.Id).SequenceEqual(mappings.Select(x => x.Id)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledWithZeroOrNegativeSinceId_ShouldReturnAllTheMappingsSortedById(int sinceId)
        {
            // Arange
            var expectedCollection =
               _existigMappings.Where(x => x.Id > sinceId).OrderBy(x => x.Id).Take(Configurations.DefaultLimit);

            // Act
            var mappings = _productCategoryMappingsService.GetMappings(sinceId: sinceId);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(mappings);
            Assert.IsTrue(mappings.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithSinceIdOutsideOfTheCategoriesIdsRange_ShouldReturnEmptyCollection()
        {
            // Arange
            int sinceId = int.MaxValue;

            // Act
            var mappings = _productCategoryMappingsService.GetMappings(sinceId: sinceId);

            // Assert
            CollectionAssert.IsEmpty(mappings);
        }
    }
}