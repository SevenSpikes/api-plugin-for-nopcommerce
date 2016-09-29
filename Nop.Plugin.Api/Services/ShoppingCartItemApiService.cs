using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;

namespace Nop.Plugin.Api.Services
{
    public class ShoppingCartItemApiService : IShoppingCartItemApiService
    {
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemsRepository;

        public ShoppingCartItemApiService(IRepository<ShoppingCartItem> shoppingCartItemsRepository)
        {
            _shoppingCartItemsRepository = shoppingCartItemsRepository;
        }

        public List<ShoppingCartItem> GetShoppingCartItems(int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                                                           DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int limit = Configurations.DefaultLimit,
                                                           int page = Configurations.DefaultPageValue)
        {
            IQueryable<ShoppingCartItem> query = GetShoppingCartItemsQuery(customerId, createdAtMin, createdAtMax,
                                                                           updatedAtMin, updatedAtMax);

            return new ApiList<ShoppingCartItem>(query, page - 1, limit);
        }

        public ShoppingCartItem GetShoppingCartItem(int id)
        {
            return _shoppingCartItemsRepository.GetById(id);
        }

        private IQueryable<ShoppingCartItem> GetShoppingCartItemsQuery(int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                                                                       DateTime? updatedAtMin = null, DateTime? updatedAtMax = null)
        {
            var query = _shoppingCartItemsRepository.TableNoTracking;

            if (customerId != null)
            {
                query = query.Where(shoppingCartItem => shoppingCartItem.CustomerId == customerId);
            }

            if (createdAtMin != null)
            {
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);
            }

            if (createdAtMax != null)
            {
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);
            }

            if (updatedAtMin != null)
            {
                query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);
            }

            if (updatedAtMax != null)
            {
                query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);
            }

            query = query.OrderBy(shoppingCartItem => shoppingCartItem.Id);

            return query;
        }
    }
}