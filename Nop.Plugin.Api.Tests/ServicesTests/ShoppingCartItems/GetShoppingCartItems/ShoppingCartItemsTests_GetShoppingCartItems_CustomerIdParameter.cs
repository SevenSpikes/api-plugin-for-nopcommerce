using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.ShoppingCartItems.GetShoppingCartItems
{
    using Nop.Core;
    using Nop.Core.Domain.Stores;

    [TestFixture]
    public class ShoppingCartItemsTests_GetShoppingCartItems_CustomerIdParameter
    {
        private IShoppingCartItemApiService _shoppingCartItemApiService;
        private List<ShoppingCartItem> _shoppingCartItems;

        [SetUp]
        public void Setup()
        {
            var randomNumber = new Random();

            _shoppingCartItems = new List<ShoppingCartItem>();

            for (int i = 1; i <= 1000; i++)
            {
                _shoppingCartItems.Add(new ShoppingCartItem()
                {
                    Id = i,
                    CustomerId = randomNumber.Next(1, 10)
                });
            }

            var shoppingCartItemsRepo = MockRepository.GenerateStub<IRepository<ShoppingCartItem>>();
            shoppingCartItemsRepo.Stub(x => x.TableNoTracking).Return(_shoppingCartItems.AsQueryable());

            var storeContext = MockRepository.GenerateStub<IStoreContext>();
            storeContext.Stub(x => x.CurrentStore).Return(new Store()
            {
                Id = 0
            });

            _shoppingCartItemApiService = new ShoppingCartItemApiService(shoppingCartItemsRepo, storeContext);
        }
        
        [Test]
        public void WhenPassedPositiveCustomerId_GivenShoppingCartItemsForThisCustomer_ShouldReturnOnlyTheShoppingCartItemsForThisCustomerSortedById()
        {
            // Arange
            int customerId = 5;
            var expectedResult = _shoppingCartItems.Where(x => x.CustomerId == customerId).OrderBy(x => x.Id).Take(Configurations.DefaultLimit);

            // Act 
            var result = _shoppingCartItemApiService.GetShoppingCartItems(customerId);

            // Assert
            Assert.IsTrue(expectedResult.Select(x => new {x.Id, x.CustomerId})
                                        .SequenceEqual(result.Select(x => new {x.Id, x.CustomerId})));
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void WhenPassedNegativeOrZeroCustomerId_ShouldReturnEmptyCollection(int customerId)
        {
            // Act 
            var result = _shoppingCartItemApiService.GetShoppingCartItems(customerId);

            // Assert
            Assert.IsEmpty(result);
        }
        
        [Test]
        public void WhenPassedNonExistentCustomerId_ShouldReturnEmptyCollection()
        {
            // Arange
            int nonExistendCustomerId = int.MaxValue;

            // Act 
            var result = _shoppingCartItemApiService.GetShoppingCartItems(nonExistendCustomerId);

            // Assert
            Assert.IsEmpty(result);
        }
    }
}