using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.OrderItemsParameters;
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
using Nop.Services.Tax;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class OrderItemsController : BaseApiController
    {
        private readonly IOrderItemApiService _orderItemApiService;
        private readonly IOrderApiService _orderApiService;
        private readonly IOrderService _orderService;
        private readonly IProductApiService _productApiService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IDTOHelper _dtoHelper;
        private readonly IProductAttributeConverter _productAttributeConverter;

        public OrderItemsController(IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService, 
            IStoreService storeService, 
            IDiscountService discountService, 
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService, 
            IOrderItemApiService orderItemApiService, 
            IOrderApiService orderApiService, 
            IOrderService orderService,
            IProductApiService productApiService, 
            IPriceCalculationService priceCalculationService, 
            ITaxService taxService,
            IPictureService pictureService, IDTOHelper dtoHelper) 
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
            _orderItemApiService = orderItemApiService;
            _orderApiService = orderApiService;
            _orderService = orderService;
            _productApiService = productApiService;
            _priceCalculationService = priceCalculationService;
            _taxService = taxService;
            _dtoHelper = dtoHelper;
        }
        
        [HttpGet]
        [ResponseType(typeof(OrderItemsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetOrderItems(int orderId, OrderItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");
            }

            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            IList<OrderItem> allOrderItemsForOrder = _orderItemApiService.GetOrderItemsForOrder(order, parameters.Limit, parameters.Page, parameters.SinceId);

            var orderItemsRootObject = new OrderItemsRootObject()
            {
                OrderItems = allOrderItemsForOrder.Select(item => _dtoHelper.PrepareOrderItemDTO(item)).ToList()
            };

            var json = _jsonFieldsSerializer.Serialize(orderItemsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [ResponseType(typeof(OrderItemsCountRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetOrderItemsCount(int orderId)
        {
            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }
            
            int orderItemsCountForOrder = _orderItemApiService.GetOrderItemsCount(order);

            var orderItemsCountRootObject = new OrderItemsCountRootObject()
            {
                Count = orderItemsCountForOrder
            };

            return Ok(orderItemsCountRootObject);
        }

        [HttpGet]
        [ResponseType(typeof(OrderItemsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetOrderItemByIdForOrder(int orderId, int orderItemId, string fields = "")
        {
            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            OrderItem orderItem = _orderService.GetOrderItemById(orderItemId);

            if (orderItem == null)
            {
                return Error(HttpStatusCode.NotFound, "order_item", "not found");
            }

            var orderItemDtos = new List<OrderItemDto>();
            orderItemDtos.Add(_dtoHelper.PrepareOrderItemDTO(orderItem));

            var orderItemsRootObject = new OrderItemsRootObject()
            {
                OrderItems = orderItemDtos
            };

            var json = _jsonFieldsSerializer.Serialize(orderItemsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof (OrderItemsRootObject))]
        public IHttpActionResult CreateOrderItem(int orderId,
            [ModelBinder(typeof (JsonModelBinder<OrderItemDto>))] Delta<OrderItemDto> orderItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            Product product = GetProduct(orderItemDelta.Dto.ProductId);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            if (product.IsRental)
            {
                if (orderItemDelta.Dto.RentalStartDateUtc == null)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_start_date_utc", "required");
                }

                if (orderItemDelta.Dto.RentalEndDateUtc == null)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_end_date_utc", "required");
                }

                if (orderItemDelta.Dto.RentalStartDateUtc > orderItemDelta.Dto.RentalEndDateUtc)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_start_date_utc", "should be before rental_end_date_utc");
                }

                if (orderItemDelta.Dto.RentalStartDateUtc < DateTime.UtcNow)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_start_date_utc", "should be a future date");
                }
            }
            
            OrderItem newOrderItem = PrepareDefaultOrderItemFromProduct(order, product);
            orderItemDelta.Merge(newOrderItem);
            
            order.OrderItems.Add(newOrderItem);

            _orderService.UpdateOrder(order);

            _customerActivityService.InsertActivity("AddNewOrderItem",
               _localizationService.GetResource("ActivityLog.AddNewOrderItem"), newOrderItem.Id);

            var orderItemsRootObject = new OrderItemsRootObject();

            orderItemsRootObject.OrderItems.Add(newOrderItem.ToDto());

            var json = _jsonFieldsSerializer.Serialize(orderItemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [ResponseType(typeof(OrderItemsRootObject))]
        public IHttpActionResult UpdateOrderItem(int orderId, int orderItemId,
          [ModelBinder(typeof(JsonModelBinder<OrderItemDto>))] Delta<OrderItemDto> orderItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            OrderItem orderItemToUpdate = _orderService.GetOrderItemById(orderItemId);

            if (orderItemToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "order_item", "not found");
            }

            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            // This is needed because those fields shouldn't be updatable. That is why we save them and after the merge set them back.
            int? productId = orderItemToUpdate.ProductId;
            DateTime? rentalStartDate = orderItemToUpdate.RentalStartDateUtc;
            DateTime? rentalEndDate = orderItemToUpdate.RentalEndDateUtc;

            orderItemDelta.Merge(orderItemToUpdate);

            orderItemToUpdate.ProductId = productId ?? 0;
            orderItemToUpdate.RentalStartDateUtc = rentalStartDate;
            orderItemToUpdate.RentalEndDateUtc = rentalEndDate;

            _orderService.UpdateOrder(order);

            _customerActivityService.InsertActivity("UpdateOrderItem",
               _localizationService.GetResource("ActivityLog.UpdateOrderItem"), orderItemToUpdate.Id);

            var orderItemsRootObject = new OrderItemsRootObject();

            orderItemsRootObject.OrderItems.Add(orderItemToUpdate.ToDto());

            var json = _jsonFieldsSerializer.Serialize(orderItemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult DeleteOrderItemById(int orderId, int orderItemId)
        {
            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            OrderItem orderItem = _orderService.GetOrderItemById(orderItemId);
            _orderService.DeleteOrderItem(orderItem);
            
            return new RawJsonActionResult("{}");
        }

        [HttpDelete]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult DeleteAllOrderItemsForOrder(int orderId)
        {
            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItemsList = order.OrderItems.ToList();

            for (int i = 0; i < orderItemsList.Count; i++)
            {
                _orderService.DeleteOrderItem(orderItemsList[i]);
            }

            return new RawJsonActionResult("{}");
        }

        private Product GetProduct(int? productId)
        {
            Product product = null;

            if (productId.HasValue)
            {
                int id = productId.Value;

                product = _productApiService.GetProductById(id);
            }
            return product;
        }

        private OrderItem PrepareDefaultOrderItemFromProduct(Order order, Product product)
        {
            var presetQty = 1;
            var presetPrice = _priceCalculationService.GetFinalPrice(product, order.Customer, decimal.Zero, true, presetQty);

            decimal taxRate;
            decimal presetPriceInclTax = _taxService.GetProductPrice(product, presetPrice, true, order.Customer, out taxRate);
            decimal presetPriceExclTax = _taxService.GetProductPrice(product, presetPrice, false, order.Customer, out taxRate);

            OrderItem orderItem = new OrderItem()
            {
                OrderItemGuid = new Guid(),
                UnitPriceExclTax = presetPriceExclTax,
                UnitPriceInclTax = presetPriceInclTax,
                PriceInclTax = presetPriceInclTax,
                PriceExclTax = presetPriceExclTax,
                OriginalProductCost = _priceCalculationService.GetProductCost(product, null),
                Quantity = presetQty,
                Product = product,
                Order = order
            };

            return orderItem;
        }
    }
}