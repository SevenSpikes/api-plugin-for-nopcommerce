using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Categories.GetCategories
{
    using Nop.Services.Stores;

    [TestFixture]
    public class CategoryApiServiceTests_GetCategories_PageParameter
    {
        private ICategoryApiService _categoryApiService;
        private List<Category> _existigCategories;

        [SetUp]
        public void Setup()
        {
            _existigCategories = new List<Category>();

            for (int i = 0; i < 1000; i++)
            {
                _existigCategories.Add(new Category()
                {
                    Id = i + 1
                });
            }

            _existigCategories[5].Deleted = true;
            _existigCategories[51].Published = false;
            _existigCategories = _existigCategories.OrderBy(x => x.Id).ToList();

            var categoryRepo = MockRepository.GenerateStub<IRepository<Category>>();
            categoryRepo.Stub(x => x.TableNoTracking).Return(_existigCategories.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();

            var storeMappingService = MockRepository.GenerateStub<IStoreMappingService>();

            _categoryApiService = new CategoryApiService(categoryRepo, productCategoryRepo, storeMappingService);
        }

        [Test]
        public void WhenCalledWithPageParameter_GivenLimitedCategoriesCollection_ShouldReturnThePartOfTheCollectionThatCorrespondsToThePage()
        {
            //Arange
            var limit = 5;
            var page = 6;
            var expectedCollection = new ApiList<Category>(_existigCategories.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var categories = _categoryApiService.GetCategories(limit: limit, page: page);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedCollection.Count(), categories.Count);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithZeroPageParameter_GivenLimitedCategoriesCollection_ShouldReturnTheFirstPage()
        {
            //Arange
            var limit = 5;
            var page = 0;
            var expectedCollection = new ApiList<Category>(_existigCategories.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var categories = _categoryApiService.GetCategories(limit: limit, page: page);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedCollection.Count(), categories.Count);
            Assert.AreEqual(_existigCategories.First().Id, categories.First().Id);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithNegativePageParameter_GivenLimitedCategoriesCollection_ShouldReturnTheFirstPage()
        {
            //Arange
            var limit = 5;
            var page = -30;
            var expectedCollection = new ApiList<Category>(_existigCategories.Where(x => !x.Deleted).AsQueryable(), page - 1, limit);

            //Act
            var categories = _categoryApiService.GetCategories(limit: limit, page: page);

            // Assert
            // Not Empty assert is a good practice when you assert something about collection. Because you can get a false positive if the collection is empty.
            CollectionAssert.IsNotEmpty(categories);
            Assert.AreEqual(expectedCollection.Count(), categories.Count);
            Assert.AreEqual(_existigCategories.First().Id, categories.First().Id);
            Assert.IsTrue(categories.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithTooBigPageParameter_GivenLimitedCategoriesCollection_ShouldReturnEmptyCollection()
        {
            //Arange
            var limit = 5;
            var page = _existigCategories.Count / limit + 100;
            
            //Act
            var categories = _categoryApiService.GetCategories(limit: limit, page: page);

            // Assert
            CollectionAssert.IsEmpty(categories);
        }
    }
}