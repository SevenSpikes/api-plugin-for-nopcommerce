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
    public class CategoryApiServiceTests_GetCategoriesCount_UpdatedParameters
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;
        private DateTime _baseDate;

        [SetUp]
        public void Setup()
        {
            _baseDate = new DateTime(2016, 2, 12);

            _existigCategories = new List<Category>()
            {
                new Category() {Id = 2, UpdatedOnUtc = _baseDate.AddMonths(2) },
                new Category() {Id = 3, UpdatedOnUtc = _baseDate.AddMonths(6) },
                new Category() {Id = 1, UpdatedOnUtc = _baseDate.AddMonths(7) },
                new Category() {Id = 4, UpdatedOnUtc = _baseDate },
                new Category() {Id = 5, UpdatedOnUtc = _baseDate.AddMonths(3) },
                new Category() {Id = 6, Deleted = true, UpdatedOnUtc = _baseDate.AddMonths(10) },
                new Category() {Id = 7, Published = false, UpdatedOnUtc = _baseDate.AddMonths(4) }
            };

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo);
        }

        [Test]
        public void WhenCalledWithUpdatedAtMinParameter_GivenSomeCategoriesUpdatedAfterThatDate_ShouldReturnTheirCount()
        {
            // Arange
            DateTime updatedAtMinDate = _baseDate.AddMonths(5);
            var expectedCollection = _existigCategories.Where(x => x.UpdatedOnUtc > updatedAtMinDate && !x.Deleted);
            var expectedCategoriesCount = expectedCollection.Count();

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(updatedAtMin: updatedAtMinDate);

            // Assert
            Assert.AreEqual(expectedCategoriesCount, categoriesCount);
        }

        [Test]
        public void WhenCalledWithUpdatedAtMinParameter_GivenAllCategoriesUpdatedBeforeThatDate_ShouldReturnZero()
        {
            // Arange
            DateTime updatedAtMinDate = _baseDate.AddMonths(7);

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(updatedAtMin: updatedAtMinDate);

            // Assert
            Assert.AreEqual(0, categoriesCount);
        }

        [Test]
        public void WhenCalledWithUpdatedAtMaxParameter_GivenSomeCategoriesUpdatedBeforeThatDate_ShouldReturnTheirCount()
        {
            // Arange
            DateTime updatedAtMaxDate = _baseDate.AddMonths(5);
            var expectedCollection =
                _existigCategories.Where(x => x.UpdatedOnUtc < updatedAtMaxDate && !x.Deleted);
            var expectedCategoriesCount = expectedCollection.Count();

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(updatedAtMax: updatedAtMaxDate);

            // Assert
            Assert.AreEqual(expectedCategoriesCount, categoriesCount);
        }

        [Test]
        public void WhenCalledWithUpdatedAtMaxParameter_GivenAllCategoriesUpdatedAfterThatDate_ShouldReturnZero()
        {
            // Arange
            DateTime updatedAtMaxDate = _baseDate.Subtract(new TimeSpan(365)); // subtract one year

            // Act
            var categoriesCount = _categoryApiService.GetCategoriesCount(updatedAtMax: updatedAtMaxDate);

            // Assert
            Assert.AreEqual(0, categoriesCount);
        }
    }
}