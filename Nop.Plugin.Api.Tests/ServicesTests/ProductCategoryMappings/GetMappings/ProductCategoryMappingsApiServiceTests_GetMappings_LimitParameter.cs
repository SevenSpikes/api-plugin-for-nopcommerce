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
    public class ProductCategoryMappingsApiServiceTests_GetMappings_LimitParameter
    {
        private IProductCategoryMappingsApiService _productCategoryMappingsService;
        private List<ProductCategory> _existigMappings;

        // Here the product and category ids does not matter so we can use the setup method.
        [SetUp]
        public void Setup()
        {
            var randomNumber = new Random();

            _existigMappings = new List<ProductCategory>();

            for (int i = 0; i < 1000; i++)
            {
                _existigMappings.Add(new ProductCategory()
                {
                   ProductId = randomNumber.Next(1, 1000),
                   CategoryId = randomNumber.Next(1, 1000),
                });
            }

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            productCategoryRepo.Stub(x => x.TableNoTracking).Return(_existigMappings.AsQueryable());

            _productCategoryMappingsService = new ProductCategoryMappingsApiService(productCategoryRepo);
        }

        [Test]
        public void WhenCalledWithLimitParameter_GivenMappingsAboveTheLimit_ShouldReturnCollectionWithCountEqualToTheLimit()
        {
            //Arange
            var expectedLimit = 5;

            //Act
            var categories = _productCategoryMappingsService.GetMappings(limit: expectedLimit);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedLimit, categories.Count);
        }

        [Test]
        public void WhenCalledWithLimitParameter_GivenMappingsBellowTheLimit_ShouldReturnCollectionWithCountEqualToTheAvailableCategories()
        {
            //Arange
            var expectedLimit = _existigMappings.Count();

            //Act
            var categories = _productCategoryMappingsService.GetMappings(limit: expectedLimit + 10);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedLimit, categories.Count);
        }

        [Test]
        public void WhenCalledWithZeroLimitParameter_GivenSomeMappings_ShouldReturnEmptyCollection()
        {
            //Arange
            var expectedLimit = 0;

            //Act
            var categories = _productCategoryMappingsService.GetMappings(limit: expectedLimit);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }

        [Test]
        public void WhenCalledWithNegativeLimitParameter_GivenSomeMappings_ShouldReturnEmptyCollection()
        {
            //Arange
            var expectedLimit = -10;

            //Act
            var categories = _productCategoryMappingsService.GetMappings(limit: expectedLimit);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }
    }
}