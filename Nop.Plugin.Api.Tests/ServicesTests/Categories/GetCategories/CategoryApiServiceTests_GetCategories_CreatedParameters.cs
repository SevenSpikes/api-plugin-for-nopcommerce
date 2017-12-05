using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories.GetCategories
{
    using Nop.Services.Stores;

    [TestFixture]
    public class CategoryApiServiceTests_GetCategories_CreatedParameters
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;
        private DateTime _baseDate;

        [SetUp]
        public void Setup()
        {
            _baseDate = new DateTime(2016, 2, 23);

            _existigCategories = new List<Category>()
            {
                new Category() {Id = 2, CreatedOnUtc = _baseDate.AddMonths(2) },
                new Category() {Id = 3, CreatedOnUtc = _baseDate.AddMonths(10) },
                new Category() {Id = 1, CreatedOnUtc = _baseDate.AddMonths(7) },
                new Category() {Id = 4, CreatedOnUtc = _baseDate },
                new Category() {Id = 5, CreatedOnUtc = _baseDate.AddMonths(3) },
                new Category() {Id = 6, Deleted = true, CreatedOnUtc = _baseDate.AddMonths(10) },
                new Category() {Id = 7, Published = false, CreatedOnUtc = _baseDate.AddMonths(4) }
            };

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
        }

        [Test]
        public void WhenCalledWithCreatedAtMinParameter_GivenSomeCategoriesCreatedAfterThatDate_ShouldReturnThemSortedById()
        {
            // Arange
            DateTime createdAtMinDate = _baseDate.AddMonths(5);
            var expectedCollection =
                _existigCategories.Where(x => x.CreatedOnUtc > createdAtMinDate && !x.Deleted).OrderBy(x => x.Id);
            var expectedCategoriesCount = expectedCollection.Count();

            // Act
            var categories = _categoryApiService.GetCategories(createdAtMin: createdAtMinDate);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedCategoriesCount, categories.Count);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithCreatedAtMinParameter_GivenAllCategoriesCreatedBeforeThatDate_ShouldReturnEmptyCollection()
        {
            // Arange
            DateTime createdAtMinDate = _baseDate.AddMonths(11);

            // Act
            var categories = _categoryApiService.GetCategories(createdAtMin: createdAtMinDate);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }
        
        [Test]
        public void WhenCalledWithCreatedAtMaxParameter_GivenSomeCategoriesCreatedBeforeThatDate_ShouldReturnThemSortedById()
        {
            // Arange
            DateTime createdAtMaxDate = _baseDate.AddMonths(5);
            var expectedCollection = _existigCategories.Where(x => x.CreatedOnUtc < createdAtMaxDate && !x.Deleted).OrderBy(x => x.Id);
            var expectedCategoriesCount = expectedCollection.Count();

            // Act
            var categories = _categoryApiService.GetCategories(createdAtMax: createdAtMaxDate);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedCategoriesCount, categories.Count);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithCreatedAtMaxParameter_GivenAllCategoriesCreatedAfterThatDate_ShouldReturnEmptyCollection()
        {
            // Arange
            DateTime createdAtMaxDate = _baseDate.Subtract(new TimeSpan(365)); // subtract one year

            // Act
            var categories = _categoryApiService.GetCategories(createdAtMax: createdAtMaxDate);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }
    }
}