using System;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    [Validator(typeof(ShoppingCartItemDtoValidator))]
    [JsonObject(Title = "shopping_cart_item")]
    public class ShoppingCartItemDto
    {
        private int? _shoppingCartTypeId;

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the price enter by a customer
        /// </summary>
        [JsonProperty("customer_entered_price")]
        public decimal? CustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the rental product start date (null if it's not a rental product)
        /// </summary>
        [JsonProperty("rental_start_date_utc")]
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental product end date (null if it's not a rental product)
        /// </summary>
        [JsonProperty("rental_end_date_utc")]
        public DateTime? RentalEndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        [JsonProperty("updated_on_utc")]
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets the log type
        /// </summary>
        [JsonProperty("shopping_cart_type")]
        public string ShoppingCartType
        {
            get
            {
                var shoppingCartTypeId = _shoppingCartTypeId;

                if (shoppingCartTypeId != null) return ((ShoppingCartType)shoppingCartTypeId).ToString();

                return null;
            }
            set
            {
                ShoppingCartType shoppingCartType;
                if (Enum.TryParse(value, true, out shoppingCartType))
                {
                    _shoppingCartTypeId = (int)shoppingCartType;
                }
                else _shoppingCartTypeId = null;
            }
        }

        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        [JsonProperty("product")]
        public ProductDto ProductDto { get; set; }

        [JsonProperty("customer_id")]
        public int? CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        [JsonProperty("customer")]
        public CustomerForShoppingCartItemDto CustomerDto { get; set; }
    }
}