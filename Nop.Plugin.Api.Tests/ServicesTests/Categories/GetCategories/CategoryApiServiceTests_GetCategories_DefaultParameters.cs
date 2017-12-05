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

    //TODO: improve using sequence equal
    [TestFixture]
    public class CategoryApiServiceTests_GetCategories_DefaultParameters
    {
        [Test]
        public void WhenCalledWithDefaultParameters_GivenNoCategoriesExist_ShouldReturnEmptyCollection()
        {
            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(new List<Category>().AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
            var categories = cut.GetCategories();

            // Assert
            Assert.AreEqual(0, categories.Count);
        }
        
        [Test]
        public void WhenCalledWithDefaultParameters_GivenOnlyDeletedCategoriesExist_ShouldReturnEmptyCollection()
        {
            var existingCategories = new List<Category>();
            existingCategories.Add(new Category() { Id = 1, Deleted = true });
            existingCategories.Add(new Category() { Id = 2, Deleted = true });

            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(existingCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
            var categories = cut.GetCategories();

            // Assert
            Assert.AreEqual(0, categories.Count);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeNotDeletedCategoriesExist_ShouldReturnThem()
        {
            var existingCategories = new List<Category>();
            existingCategories.Add(new Category() { Id = 1 });
            existingCategories.Add(new Category() { Id = 2, Deleted = true });
            existingCategories.Add(new Category() { Id = 3 });

            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(existingCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
            var categories = cut.GetCategories();

            // Assert
            Assert.AreEqual(2, categories.Count);
            Assert.AreEqual(existingCategories[0].Id, categories[0].Id);
            Assert.AreEqual(existingCategories[2].Id, categories[1].Id);
        }

        [Test]
        public void WhenCalledWithDefaultParameters_GivenSomeCategoriesExist_ShouldReturnThemSortedById()
        {
            var existingCategories = new List<Category>();
            existingCategories.Add(new Category() { Id = 2 });
            existingCategories.Add(new Category() { Id = 3 });
            existingCategories.Add(new Category() { Id = 1 });

            var sortedIds = new List<int>() {1,2,3};

            // Arange
            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(existingCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            // Act
            var cut = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
            var categories = cut.GetCategories();

            // Assert
            Assert.AreEqual(sortedIds[0], categories[0].Id);
            Assert.AreEqual(sortedIds[1], categories[1].Id);
            Assert.AreEqual(sortedIds[2], categories[2].Id);
        }
    }
}