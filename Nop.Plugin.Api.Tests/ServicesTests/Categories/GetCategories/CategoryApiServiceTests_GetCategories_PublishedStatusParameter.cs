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
    public class CategoryApiServiceTests_GetCategories_PublishedStatusParameter
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;

        [SetUp]
        public void Setup()
        {
            _existigCategories = new List<Category>()
            {
                new Category() {Id = 2, Published = true },
                new Category() {Id = 3, Published = true  },
                new Category() {Id = 1, Published = false  },
                new Category() {Id = 4, Published = true  },
                new Category() {Id = 5, Published = true  },
                new Category() {Id = 6, Deleted = true, Published = true  },
                new Category() {Id = 7, Published = false }
            };

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
        }

        [Test]
        public void WhenAskForOnlyThePublishedCategories_ShouldReturnOnlyThePublishedOnesSortedById()
        {
            // Arange
            var expectedCollection = _existigCategories.Where(x => x.Published && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var categories = _categoryApiService.GetCategories(publishedStatus: true);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenAskForOnlyTheUnpublishedCategories_ShouldReturnOnlyTheUnpublishedOnesSortedById()
        {
            // Arange
            var expectedCollection = _existigCategories.Where(x => !x.Published && !x.Deleted).OrderBy(x => x.Id);

            // Act
            var categories = _categoryApiService.GetCategories(publishedStatus: false);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }
    }
}