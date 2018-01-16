using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation.Results;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Api.Controllers
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Nop.Plugin.Api.DTOs.Errors;
    using Nop.Plugin.Api.JSON.Serializers;

    [ApiAuthorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderApiService _orderApiService;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShippingService _shippingService;
        private readonly IDTOHelper _dtoHelper;        
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IStoreContext _storeContext;
        private readonly IFactory<Order> _factory;

        // We resolve the order settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private OrderSettings _orderSettings;

        private OrderSettings OrderSettings
        {
            get
            {
                if (_orderSettings == null)
                {
                    _orderSettings = EngineContext.Current.Resolve<OrderSettings>();
                }

                return _orderSettings;
            }
        }

        public OrdersController(IOrderApiService orderApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductService productService,
            IFactory<Order> factory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IShippingService shippingService,
            IPictureService pictureService,
            IDTOHelper dtoHelper,
            IProductAttributeConverter productAttributeConverter)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                 storeService, discountService, customerActivityService, localizationService,pictureService)
        {
            _orderApiService = orderApiService;
            _factory = factory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _dtoHelper = dtoHelper;
            _productService = productService;
            _productAttributeConverter = productAttributeConverter;
        }

        /// <summary>
        /// Receive a list of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrders(OrdersParametersModel parameters)
        {
            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid limit parameter");
            }

            var storeId = _storeContext.CurrentStore.Id;

            IList<Order> orders = _orderApiService.GetOrders(parameters.Ids, parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                parameters.CustomerId, storeId);

            IList<OrderDto> ordersAsDtos = orders.Select(x => _dtoHelper.PrepareOrderDTO(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/count")]
        [ProducesResponseType(typeof(OrdersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersCount(OrdersCountParametersModel parameters)
        {
            var storeId = _storeContext.CurrentStore.Id;

            int ordersCount = _orderApiService.GetOrdersCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status,
                                                              parameters.PaymentStatus, parameters.ShippingStatus, parameters.CustomerId, storeId);

            var ordersCountRootObject = new OrdersCountRootObject()
            {
                Count = ordersCount
            };

            return Ok(ordersCountRootObject);
        }

        /// <summary>
        /// Retrieve order by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            Order order = _orderApiService.GetOrderById(id);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var ordersRootObject = new OrdersRootObject();

            OrderDto orderDto = _dtoHelper.PrepareOrderDTO(order);
            ordersRootObject.Orders.Add(orderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve all orders for customer
        /// </summary>
        /// <param name="customer_id">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/customer/{customer_id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersByCustomerId(int customer_id)
        {
            IList<OrderDto> ordersForCustomer = _orderApiService.GetOrdersByCustomerId(customer_id).Select(x => _dtoHelper.PrepareOrderDTO(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersForCustomer
            };

            return Ok(ordersRootObject);
        }

        [HttpPost]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We doesn't have to check for value because this is done by the order validator.
            Customer customer = _customerService.GetCustomerById(orderDelta.Dto.CustomerId.Value);
            
            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            bool shippingRequired = false;

            if (orderDelta.Dto.OrderItemDtos != null)
            {
                bool shouldReturnError = ValidateEachOrderItem(orderDelta.Dto.OrderItemDtos);

                if (shouldReturnError)
                {
                    return Error(HttpStatusCode.BadRequest);
                }

                shouldReturnError = AddOrderItemsToCart(orderDelta.Dto.OrderItemDtos, customer, orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id);

                if (shouldReturnError)
                {
                    return Error(HttpStatusCode.BadRequest);
                }

                shippingRequired = IsShippingAddressRequired(orderDelta.Dto.OrderItemDtos);
            }

            if (shippingRequired)
            {
                bool isValid = true;

                isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName,
                                            orderDelta.Dto.ShippingMethod,
                                            orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id,
                                            customer, 
                                            BuildShoppingCartItemsFromOrderItemDtos(orderDelta.Dto.OrderItemDtos.ToList(), 
                                                                                    customer.Id, 
                                                                                    orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id));

                isValid &= ValidateAddress(orderDelta.Dto.ShippingAddress, "shipping_address");

                if (!isValid)
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            if (!OrderSettings.DisableBillingAddressCheckoutStep)
            {
                bool isValid = ValidateAddress(orderDelta.Dto.BillingAddress, "billing_address");

                if (!isValid)
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            Order newOrder = _factory.Initialize();
            orderDelta.Merge(newOrder);

            customer.BillingAddress = newOrder.BillingAddress;
            customer.ShippingAddress = newOrder.ShippingAddress;
            // If the customer has something in the cart it will be added too. Should we clear the cart first? 
            newOrder.Customer = customer;

            // The default value will be the currentStore.id, but if it isn't passed in the json we need to set it by hand.
            if (!orderDelta.Dto.StoreId.HasValue)
            {
                newOrder.StoreId = _storeContext.CurrentStore.Id;
            }
            
            PlaceOrderResult placeOrderResult = PlaceOrder(newOrder, customer);

            if (!placeOrderResult.Success)
            {
                foreach (var error in placeOrderResult.Errors)
                {
                    ModelState.AddModelError("order placement", error);
                }

                return Error(HttpStatusCode.BadRequest);
            }

            _customerActivityService.InsertActivity("AddNewOrder",
                 _localizationService.GetResource("ActivityLog.AddNewOrder"), newOrder.Id);

            var ordersRootObject = new OrdersRootObject();

            OrderDto placedOrderDto = _dtoHelper.PrepareOrderDTO(placeOrderResult.PlacedOrder);

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteOrder(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }
            
            Order orderToDelete = _orderApiService.GetOrderById(id);

            if (orderToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            _orderProcessingService.DeleteOrder(orderToDelete);

            //activity log
            _customerActivityService.InsertActivity("DeleteOrder", _localizationService.GetResource("ActivityLog.DeleteOrder"), orderToDelete.Id);

            return new RawJsonActionResult("{}");
        }

        [HttpPut]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            Order currentOrder = _orderApiService.GetOrderById(int.Parse(orderDelta.Dto.Id));

            if (currentOrder == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            Customer customer = currentOrder.Customer;

            bool shippingRequired = currentOrder.OrderItems.Any(item => !item.Product.IsFreeShipping);

            if (shippingRequired)
            {
                bool isValid = true;

                if (!string.IsNullOrEmpty(orderDelta.Dto.ShippingRateComputationMethodSystemName) ||
                    !string.IsNullOrEmpty(orderDelta.Dto.ShippingMethod))
                {
                    var storeId = orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id;

                    isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName ?? currentOrder.ShippingRateComputationMethodSystemName,
                        orderDelta.Dto.ShippingMethod, 
                        storeId,
                        customer, BuildShoppingCartItemsFromOrderItems(currentOrder.OrderItems.ToList(), customer.Id, storeId));
                }

                if (orderDelta.Dto.ShippingAddress != null)
                {
                    isValid &= ValidateAddress(orderDelta.Dto.ShippingAddress, "shipping_address");
                }

                if (isValid)
                {
                    currentOrder.ShippingMethod = orderDelta.Dto.ShippingMethod;
                }
                else
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            if (!OrderSettings.DisableBillingAddressCheckoutStep && orderDelta.Dto.BillingAddress != null)
            {
                bool isValid = ValidateAddress(orderDelta.Dto.BillingAddress, "billing_address");

                if (!isValid)
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            orderDelta.Merge(currentOrder);
            
            customer.BillingAddress = currentOrder.BillingAddress;
            customer.ShippingAddress = currentOrder.ShippingAddress;

            _orderService.UpdateOrder(currentOrder);

            _customerActivityService.InsertActivity("UpdateOrder",
                 _localizationService.GetResource("ActivityLog.UpdateOrder"), currentOrder.Id);

            var ordersRootObject = new OrdersRootObject();

            OrderDto placedOrderDto = _dtoHelper.PrepareOrderDTO(currentOrder);
            placedOrderDto.ShippingMethod = orderDelta.Dto.ShippingMethod;

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private bool SetShippingOption(string shippingRateComputationMethodSystemName, string shippingOptionName, int storeId, Customer customer, List<ShoppingCartItem> shoppingCartItems)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(shippingRateComputationMethodSystemName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_rate_computation_method_system_name",
                    "Please provide shipping_rate_computation_method_system_name");
            }
            else if (string.IsNullOrEmpty(shippingOptionName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_option_name", "Please provide shipping_option_name");
            }
            else
            {
                GetShippingOptionResponse shippingOptionResponse = _shippingService.GetShippingOptions(shoppingCartItems, customer.ShippingAddress, customer,
                        shippingRateComputationMethodSystemName, storeId);

                var shippingOptions = new List<ShippingOption>();

                if (shippingOptionResponse.Success)
                {
                    shippingOptions = shippingOptionResponse.ShippingOptions.ToList();

                    ShippingOption shippingOption = shippingOptions
                        .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(shippingOptionName, StringComparison.InvariantCultureIgnoreCase));
                    
                    _genericAttributeService.SaveAttribute(customer,
                        SystemCustomerAttributeNames.SelectedShippingOption,
                        shippingOption, storeId);
                }
                else
                {
                    isValid = false;

                    foreach (var errorMessage in shippingOptionResponse.Errors)
                    {
                        ModelState.AddModelError("shipping_option", errorMessage);
                    }
                }
            }

            return isValid;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItems(List<OrderItem> orderItems, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItems)
            {
                shoppingCartItems.Add(new ShoppingCartItem()
                {
                    ProductId = orderItem.ProductId,
                    CustomerId = customerId,
                    Quantity = orderItem.Quantity,
                    RentalStartDateUtc = orderItem.RentalStartDateUtc,
                    RentalEndDateUtc = orderItem.RentalEndDateUtc,
                    StoreId = storeId,
                    Product = orderItem.Product,
                    ShoppingCartType = ShoppingCartType.ShoppingCart
                });
            }

            return shoppingCartItems;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItemDtos(List<OrderItemDto> orderItemDtos, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItemDtos)
            {
                shoppingCartItems.Add(new ShoppingCartItem()
                {
                    ProductId = orderItem.ProductId.Value, // required field
                    CustomerId = customerId,
                    Quantity = orderItem.Quantity ?? 1,
                    RentalStartDateUtc = orderItem.RentalStartDateUtc,
                    RentalEndDateUtc = orderItem.RentalEndDateUtc,
                    StoreId = storeId,
                    Product = _productService.GetProductById(orderItem.ProductId.Value),
                    ShoppingCartType = ShoppingCartType.ShoppingCart
                });
            }

            return shoppingCartItems;
        }

        private PlaceOrderResult PlaceOrder(Order newOrder, Customer customer)
        {
            var processPaymentRequest = new ProcessPaymentRequest();

            processPaymentRequest.StoreId = newOrder.StoreId;
            processPaymentRequest.CustomerId = customer.Id;
            processPaymentRequest.PaymentMethodSystemName = newOrder.PaymentMethodSystemName;

            PlaceOrderResult placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);

            return placeOrderResult;
        }

        private bool ValidateEachOrderItem(ICollection<OrderItemDto> orderItems)
        {
            bool shouldReturnError = false;

            foreach (var orderItem in orderItems)
            {
                var orderItemDtoValidator = new OrderItemDtoValidator("post", null);
                ValidationResult validation = orderItemDtoValidator.Validate(orderItem);

                if (validation.IsValid)
                {
                    Product product = _productService.GetProductById(orderItem.ProductId.Value);

                    if (product == null)
                    {
                        ModelState.AddModelError("order_item.product", string.Format("Product not found for order_item.product_id = {0}", orderItem.ProductId));
                        shouldReturnError = true;
                    }
                }
                else
                {
                    foreach (var error in validation.Errors)
                    {
                        ModelState.AddModelError("order_item", error.ErrorMessage);
                    }

                    shouldReturnError = true;
                }
            }

            return shouldReturnError;
        }

        private bool IsShippingAddressRequired(ICollection<OrderItemDto> orderItems)
        {
            bool shippingAddressRequired = false;

            foreach (var orderItem in orderItems)
            {
                Product product = _productService.GetProductById(orderItem.ProductId.Value);

                shippingAddressRequired |= product.IsShipEnabled;
            }

            return shippingAddressRequired;
        }

        private bool AddOrderItemsToCart(ICollection<OrderItemDto> orderItems, Customer customer, int storeId)
        {
            bool shouldReturnError = false;

            foreach (var orderItem in orderItems)
            {
                Product product = _productService.GetProductById(orderItem.ProductId.Value);

                if (!product.IsRental)
                {
                    orderItem.RentalStartDateUtc = null;
                    orderItem.RentalEndDateUtc = null;
                }

                string attributesXml = _productAttributeConverter.ConvertToXml(orderItem.Attributes, product.Id);                

                IList<string> errors = _shoppingCartService.AddToCart(customer, product,
                    ShoppingCartType.ShoppingCart, storeId,attributesXml,
                    0M, orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                    orderItem.Quantity ?? 1);

                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("order", error);
                    }

                    shouldReturnError = true;
                }
            }

            return shouldReturnError;
        }
        
        private bool ValidateAddress(AddressDto address, string addressKind)
        {
            bool addressValid = true;

            if (address == null)
            {
                ModelState.AddModelError(addressKind, string.Format("{0} address required", addressKind));
                addressValid = false;
            }
            else
            {
                var addressValidator = new AddressDtoValidator();
                ValidationResult validationResult = addressValidator.Validate(address);

                foreach (var validationFailure in validationResult.Errors)
                {
                    ModelState.AddModelError(addressKind, validationFailure.ErrorMessage);
                }

                addressValid = validationResult.IsValid;
            }

            return addressValid;
        }
    }
}