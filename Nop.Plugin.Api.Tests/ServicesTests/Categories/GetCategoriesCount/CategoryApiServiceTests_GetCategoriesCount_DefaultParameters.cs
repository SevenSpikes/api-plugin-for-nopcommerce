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
    public class CategoryApiServiceTests_GetCategoriesCount_DefaultParameters
    {
        [Test]
        public void WhenCalledWithDefaultParameters_GivenNoCategoriesExist_ShouldReturnZero()
        {
            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(new List<Category>().AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo);
            var categoriesCount = cut.GetCategoriesCount();

            // Assert
            Assert.AreEqual(0, categoriesCount);
        }
        
        [Test]
        public void WhenCalledWithDefaultParameters_GivenOnlyDeletedCategoriesExist_ShouldReturnZero()
        {
            var existingCategories = new List<Category>();
            existingCategories.Add(new Category() { Id = 1, Deleted = true });
            existingCategories.Add(new Category() { Id = 2, Deleted = true });

            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(existingCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo);
            var countResult = cut.GetCategoriesCount();

            // Assert
            Assert.AreEqual(0, countResult);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeNotDeletedCategoriesExist_ShouldReturnTheirCount()
        {
            var existingCategories = new List<Category>();
            existingCategories.Add(new Category() { Id = 1 });
            existingCategories.Add(new Category() { Id = 2, Deleted = true });
            existingCategories.Add(new Category() { Id = 3 });

            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(existingCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo);
            var countResult = cut.GetCategoriesCount();

            // Assert
            Assert.AreEqual(2, countResult);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeCategoriesExist_ShouldReturnTheirCount()
        {
            var existingCategories = new List<Category>();
            existingCategories.Add(new Category() { Id = 2, Published = false });
            existingCategories.Add(new Category() { Id = 3 });
            existingCategories.Add(new Category() { Id = 1 });
            
            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(existingCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo);
            var countResult = cut.GetCategoriesCount();

            // Assert
            Assert.AreEqual(existingCategories.Count, countResult);
        }
    }
}
