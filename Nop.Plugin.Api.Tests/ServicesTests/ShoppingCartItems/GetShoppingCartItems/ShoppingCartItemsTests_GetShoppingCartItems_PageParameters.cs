using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.ShoppingCartItems.GetShoppingCartItems
{
    using Nop.Core;
    using Nop.Core.Domain.Stores;

    [TestFixture]
    public class ShoppingCartItemsTests_GetShoppingCartItems_PageParameters
    {
        private IShoppingCartItemApiService _shoppingCartItemApiService;
        private List<ShoppingCartItem> _existigShoppingCartItems;

        [SetUp]
        public void Setup()
        {
            _existigShoppingCartItems = new List<ShoppingCartItem>();

            for (int i = 1; i <= 1000; i++)
            {
                _existigShoppingCartItems.Add(new ShoppingCartItem()
                {
                    Id = i
                });
            }

            var shoppingCartItemRepo = MockRepository.GenerateStub<IRepository<ShoppingCartItem>>();
            shoppingCartItemRepo.Stub(x => x.TableNoTracking).Return(_existigShoppingCartItems.AsQueryable());

            var storeContext = MockRepository.GenerateStub<IStoreContext>();
            storeContext.Stub(x => x.CurrentStore).Return(new Store()
            {
                Id = 0
            });

            _shoppingCartItemApiService = new ShoppingCartItemApiService(shoppingCartItemRepo, storeContext);
        }

        [Test]
        public void WhenCalledWithPageParameter_ShouldReturnThePartOfTheCollectionThatCorrespondsToThePage()
        {
            //Arange
            var limit = 5;
            var page = 6;
            var expectedCollection = new ApiList<ShoppingCartItem>(_existigShoppingCartItems.AsQueryable(), page - 1, limit);

            //Act
            var result = _shoppingCartItemApiService.GetShoppingCartItems(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(expectedCollection.Count(), result.Count);
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenCalledWithZeroOrNegativePageParameter_ShouldReturnTheFirstPage(int page)
        {
            //Arange
            var limit = 5;
            var expectedCollection = new ApiList<ShoppingCartItem>(_existigShoppingCartItems.AsQueryable(), page - 1, limit);

            //Act
            var result = _shoppingCartItemApiService.GetShoppingCartItems(limit: limit, page: page);

            // Assert
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(expectedCollection.Count(), result.Count);
            Assert.AreEqual(_existigShoppingCartItems.First().Id, result.First().Id);
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void WhenCalledWithTooBigPageParameter_ShouldReturnEmptyCollection()
        {
            //Arange
            var limit = 5;
            var page = _existigShoppingCartItems.Count / limit + 100;
            
            //Act
            var result = _shoppingCartItemApiService.GetShoppingCartItems(limit: limit, page: page);

            // Assert
            CollectionAssert.IsEmpty(result);
        }
    }
}