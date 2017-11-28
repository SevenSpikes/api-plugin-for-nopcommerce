﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ShoppingCartsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Plugin.Api.Helpers;
using Nop.Core;

namespace Nop.Plugin.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ShoppingCartItemsController : BaseApiController
    {
        private readonly IShoppingCartItemApiService _shoppingCartItemApiService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IFactory<ShoppingCartItem> _factory;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IDTOHelper _dtoHelper;
        private readonly IStoreContext _storeContext;

        public ShoppingCartItemsController(IShoppingCartItemApiService shoppingCartItemApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IFactory<ShoppingCartItem> factory,
            IPictureService pictureService,
            IProductAttributeConverter productAttributeConverter,
            IDTOHelper dtoHelper,
            IStoreContext storeContext)
            : base(jsonFieldsSerializer,
                 aclService,
                 customerService,
                 storeMappingService,
                 storeService,
                 discountService,
                 customerActivityService,
                 localizationService,
                 pictureService)
        {
            _shoppingCartItemApiService = shoppingCartItemApiService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _factory = factory;
            _productAttributeConverter = productAttributeConverter;
            _dtoHelper = dtoHelper;
            _storeContext = storeContext;
        }

        /// <summary>
        /// Receive a list of all shopping cart items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetShoppingCartItems(ShoppingCartItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId: null,
                                                                                                         createdAtMin: parameters.CreatedAtMin,
                                                                                                         createdAtMax: parameters.CreatedAtMax,
                                                                                                         updatedAtMin: parameters.UpdatedAtMin,
                                                                                                         updatedAtMax: parameters.UpdatedAtMax,
                                                                                                         limit: parameters.Limit,
                                                                                                         page: parameters.Page);

            List<ShoppingCartItemDto> shoppingCartItemsDtos = shoppingCartItems.Select(shoppingCartItem =>
            {
               return _dtoHelper.PrepareShoppingCartItemDTO(shoppingCartItem);
            }).ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject()
            {
                ShoppingCartItems = shoppingCartItemsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a list of all shopping cart items by customer id
        /// </summary>
        /// <param name="customerId">Id of the customer whoes shopping cart items you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items/{customerId}")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetShoppingCartItemsByCustomerId(int customerId, ShoppingCartItemsForCustomerParametersModel parameters)
        {
            if (customerId <= Configurations.DefaultCustomerId)
            {
                return Error(HttpStatusCode.BadRequest, "customer_id", "invalid customer_id");
            }

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId,
                                                                                                         parameters.CreatedAtMin,
                                                                                                         parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                                                         parameters.UpdatedAtMax, parameters.Limit,
                                                                                                         parameters.Page);

            if (shoppingCartItems == null)
            {
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
            }

            List<ShoppingCartItemDto> shoppingCartItemsDtos = shoppingCartItems.Select(shoppingCartItem =>
            {
                return _dtoHelper.PrepareShoppingCartItemDTO(shoppingCartItem);
            }).ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject()
            {
                ShoppingCartItems = shoppingCartItemsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/shopping_cart_items")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), 422)]
        public IActionResult CreateShoppingCartItem([ModelBinder(typeof(JsonModelBinder<ShoppingCartItemDto>))] Delta<ShoppingCartItemDto> shoppingCartItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            ShoppingCartItem newShoppingCartItem = _factory.Initialize();
            shoppingCartItemDelta.Merge(newShoppingCartItem);

            // We know that the product id and customer id will be provided because they are required by the validator.
            // TODO: validate
            Product product = _productService.GetProductById(newShoppingCartItem.ProductId);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            Customer customer = _customerService.GetCustomerById(newShoppingCartItem.CustomerId);

            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            ShoppingCartType shoppingCartType = (ShoppingCartType)Enum.Parse(typeof(ShoppingCartType), shoppingCartItemDelta.Dto.ShoppingCartType);

            if (!product.IsRental)
            {
                newShoppingCartItem.RentalStartDateUtc = null;
                newShoppingCartItem.RentalEndDateUtc = null;
            }

            string attributesXml =_productAttributeConverter.ConvertToXml(shoppingCartItemDelta.Dto.Attributes, product.Id);

            int currentStoreId = _storeContext.CurrentStore.Id;

            IList<string> warnings = _shoppingCartService.AddToCart(customer, product, shoppingCartType, currentStoreId, attributesXml, 0M,
                                        newShoppingCartItem.RentalStartDateUtc, newShoppingCartItem.RentalEndDateUtc,
                                        shoppingCartItemDelta.Dto.Quantity ?? 1);

            if (warnings.Count > 0)
            {
                foreach (var warning in warnings)
                {
                    ModelState.AddModelError("shopping cart item", warning);
                }

                return Error(HttpStatusCode.BadRequest);
            }
            else {
                // the newly added shopping cart item should be the last one
                newShoppingCartItem = customer.ShoppingCartItems.LastOrDefault();
            }

            // Preparing the result dto of the new product category mapping
            ShoppingCartItemDto newShoppingCartItemDto = _dtoHelper.PrepareShoppingCartItemDTO(newShoppingCartItem);

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject();

            shoppingCartsRootObject.ShoppingCartItems.Add(newShoppingCartItemDto);

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/shopping_cart_items/{id}")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), 422)]
        public IActionResult UpdateShoppingCartItem([ModelBinder(typeof(JsonModelBinder<ShoppingCartItemDto>))] Delta<ShoppingCartItemDto> shoppingCartItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We kno that the id will be valid integer because the validation for this happens in the validator which is executed by the model binder.
            ShoppingCartItem shoppingCartItemForUpdate =
                _shoppingCartItemApiService.GetShoppingCartItem(int.Parse(shoppingCartItemDelta.Dto.Id));

            if (shoppingCartItemForUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
            }

            shoppingCartItemDelta.Merge(shoppingCartItemForUpdate);            

            if (!shoppingCartItemForUpdate.Product.IsRental)
            {
                shoppingCartItemForUpdate.RentalStartDateUtc = null;
                shoppingCartItemForUpdate.RentalEndDateUtc = null;
            }

            if (shoppingCartItemDelta.Dto.Attributes != null)
            {
                shoppingCartItemForUpdate.AttributesXml = _productAttributeConverter.ConvertToXml(shoppingCartItemDelta.Dto.Attributes, shoppingCartItemForUpdate.Product.Id);
            }

            // The update time is set in the service.
            var warnings = _shoppingCartService.UpdateShoppingCartItem(shoppingCartItemForUpdate.Customer, shoppingCartItemForUpdate.Id,
                shoppingCartItemForUpdate.AttributesXml, shoppingCartItemForUpdate.CustomerEnteredPrice,
                shoppingCartItemForUpdate.RentalStartDateUtc, shoppingCartItemForUpdate.RentalEndDateUtc,
                shoppingCartItemForUpdate.Quantity);

            if (warnings.Count > 0)
            {
                foreach (var warning in warnings)
                {
                    ModelState.AddModelError("shopping cart item", warning);
                }

                return Error(HttpStatusCode.BadRequest);
            }
            else
            {
                shoppingCartItemForUpdate = _shoppingCartItemApiService.GetShoppingCartItem(shoppingCartItemForUpdate.Id);
            }

            // Preparing the result dto of the new product category mapping
            ShoppingCartItemDto newShoppingCartItemDto = _dtoHelper.PrepareShoppingCartItemDTO(shoppingCartItemForUpdate);

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject();

            shoppingCartsRootObject.ShoppingCartItems.Add(newShoppingCartItemDto);

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/shopping_cart_items/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteShoppingCartItem(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            ShoppingCartItem shoppingCartItemForDelete = _shoppingCartItemApiService.GetShoppingCartItem(id);

            if (shoppingCartItemForDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
            }

            _shoppingCartService.DeleteShoppingCartItem(shoppingCartItemForDelete);

            //activity log
            _customerActivityService.InsertActivity("DeleteShoppingCartItem", _localizationService.GetResource("ActivityLog.DeleteShoppingCartItem"), shoppingCartItemForDelete.Id);

            return new RawJsonActionResult("{}");
        }
    }
}