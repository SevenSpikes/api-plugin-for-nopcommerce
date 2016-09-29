using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories.GetCategoriesCount
{
    [TestFixture]
    public class CategoryApiServiceTests_GetCategoriesCount_CreatedParameters
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

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo);
        }

        [Test]
        public void WhenCalledWithCreatedAtMinParameter_GivenSomeCategoriesCreatedAfterThatDate_ShouldReturnTheirCount()
        {
            // Arange
            DateTime createdAtMinDate = _baseDate.AddMonths(5);
            var expectedCollection =
                _existigCategories.Where(x => x.CreatedOnUtc > createdAtMinDate && !x.Deleted);
            var expectedCategoriesCount = expectedCollection.Count();

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(createdAtMin: createdAtMinDate);

            // Assert
            Assert.AreEqual(expectedCategoriesCount, categoriesCount);
        }

        [Test]
        public void WhenCalledWithCreatedAtMinParameter_GivenAllCategoriesCreatedBeforeThatDate_ShouldReturnZero()
        {
            // Arange
            DateTime createdAtMinDate = _baseDate.AddMonths(11);

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(createdAtMin: createdAtMinDate);

            // Assert
            Assert.AreEqual(0, categoriesCount);
        }

        [Test]
        public void WhenCalledWithCreatedAtMaxParameter_GivenSomeCategoriesCreatedBeforeThatDate_ShouldReturnTheirCount()
        {
            // Arange
            DateTime createdAtMaxDate = _baseDate.AddMonths(5);
            var expectedCollection = _existigCategories.Where(x => x.CreatedOnUtc < createdAtMaxDate && !x.Deleted);
            var expectedCategoriesCount = expectedCollection.Count();

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(createdAtMax: createdAtMaxDate);

            // Assert
            Assert.AreEqual(expectedCategoriesCount, categoriesCount);
        }

        [Test]
        public void WhenCalledWithCreatedAtMaxParameter_GivenAllCategoriesCreatedAfterThatDate_ShouldReturnZero()
        {
            // Arange
            DateTime createdAtMaxDate = _baseDate.Subtract(new TimeSpan(365)); // subtract one year

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(createdAtMax: createdAtMaxDate);

            // Assert
            Assert.AreEqual(0, categoriesCount);
        }
    }
}