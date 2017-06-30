using System;
using Microsoft.AspNet.WebHooks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Services;
using Nop.Services.Events;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.MappingExtensions;

namespace Nop.Plugin.Api.WebHooks
{
    public class WebHookEventConsumer : IConsumer<EntityInserted<Customer>>,
        IConsumer<EntityUpdated<Customer>>,
        IConsumer<EntityInserted<Product>>,
        IConsumer<EntityUpdated<Product>>,
        IConsumer<EntityInserted<Category>>,
        IConsumer<EntityUpdated<Category>>,
        IConsumer<EntityInserted<Order>>,
        IConsumer<EntityUpdated<Order>>
    {
        private IWebHookManager _webHookManager;
        private ICustomerApiService _customerApiService;
        private IDTOHelper _dtoHelper;

        public WebHookEventConsumer()
        {
            IWebHookService webHookService = EngineContext.Current.ContainerManager.Resolve<IWebHookService>();
            _customerApiService = EngineContext.Current.ContainerManager.Resolve<ICustomerApiService>();
            _dtoHelper = EngineContext.Current.ContainerManager.Resolve<IDTOHelper>();

            _webHookManager = webHookService.GetHookManager();
        }

        public void HandleEvent(EntityInserted<Customer> eventMessage)
        {
            // There is no need to send webhooks for guest customers or customers that are in the Trade role
            if (eventMessage.Entity.IsGuest() || eventMessage.Entity.IsInCustomerRole(BLBSettings.TradeCustomerRoleSystemName))
            {
                return;
            }

            CustomerDto customer = _customerApiService.GetCustomerById(eventMessage.Entity.Id);
            _webHookManager.NotifyAllAsync(WebHookNames.CustomerCreated, new { Item = customer });
        }

        public void HandleEvent(EntityUpdated<Customer> eventMessage)
        {
            // There is no need to send webhooks for guest customers or customers that are in the Trade role
            if (eventMessage.Entity.IsGuest())
            {
                return;
            }

            // In nopCommerce the Customer, Product, Category and Order entities are not deleted.
            // Instead the Deleted property of the entity is set to true.
            string webhookEvent = WebHookNames.CustomerUpdated;

            if (eventMessage.Entity.IsInCustomerRole(BLBSettings.TradeCustomerRoleSystemName))
            {
                var createdDaysAgo = DateTime.UtcNow.Subtract(eventMessage.Entity.CreatedOnUtc).Days;
                // since initially a customer could be created without any roles we don't know if it will be Trade or not and we process it
                // So if the customer is updated within a date after its creation we ensure that it is removed
                // after that we simply don't send a hook as there will be too many hooks
                if (createdDaysAgo < 1)
                {
                    // customers that are recently created and are now Trade accounts should be removed
                    webhookEvent = WebHookNames.CustomerDeleted;
                }
                else
                {
                    // no need to spam with hooks for Trade accounts updates
                    return;
                }
            }

            CustomerDto customer = _customerApiService.GetCustomerById(eventMessage.Entity.Id, true);

            if(customer.Deleted == true)
            {
                webhookEvent = WebHookNames.CustomerDeleted;
            }

            _webHookManager.NotifyAllAsync(webhookEvent, new { Item = customer });
        }

        public void HandleEvent(EntityInserted<Product> eventMessage)
        {
            ProductDto productDto = _dtoHelper.PrepareProductDTO(eventMessage.Entity);

            _webHookManager.NotifyAllAsync(WebHookNames.ProductCreated, new { Item = productDto });
        }

        public void HandleEvent(EntityUpdated<Product> eventMessage)
        {
            ProductDto productDto = _dtoHelper.PrepareProductDTO(eventMessage.Entity);

            string webhookEvent = WebHookNames.ProductUpdated;

            if (productDto.Deleted == true)
            {
                webhookEvent = WebHookNames.ProductDeleted;
            }

            _webHookManager.NotifyAllAsync(webhookEvent, new { Item = productDto });
        }

        public void HandleEvent(EntityInserted<Category> eventMessage)
        {
            CategoryDto categoryDto = _dtoHelper.PrepareCategoryDTO(eventMessage.Entity);

            _webHookManager.NotifyAllAsync(WebHookNames.CategoryCreated, new { Item = categoryDto });
        }

        public void HandleEvent(EntityUpdated<Category> eventMessage)
        {
            CategoryDto categoryDto = _dtoHelper.PrepareCategoryDTO(eventMessage.Entity);

            string webhookEvent = WebHookNames.CategoryUpdated;

            if (categoryDto.Deleted == true)
            {
                webhookEvent = WebHookNames.CategoryDeleted;
            }

            _webHookManager.NotifyAllAsync(webhookEvent, new { Item = categoryDto });
        }

        public void HandleEvent(EntityInserted<Order> eventMessage)
        {
            // we need to handle only BLB orders
            if(eventMessage.Entity.StoreId != BLBSettings.BLBStoreId)
                return;

            OrderDto orderDto = _dtoHelper.PrepareOrderDTO(eventMessage.Entity);

            _webHookManager.NotifyAllAsync(WebHookNames.OrderCreated, new { Item = orderDto });
        }

        public void HandleEvent(EntityUpdated<Order> eventMessage)
        {
            // we need to handle only BLB orders
            if (eventMessage.Entity.StoreId != BLBSettings.BLBStoreId)
                return;

            OrderDto orderDto = _dtoHelper.PrepareOrderDTO(eventMessage.Entity);

            string webhookEvent = WebHookNames.OrderUpdated;

            if (orderDto.Deleted == true)
            {
                webhookEvent = WebHookNames.OrderDeleted;
            }

            _webHookManager.NotifyAllAsync(webhookEvent, new { Item = orderDto });
        }
    }
}
