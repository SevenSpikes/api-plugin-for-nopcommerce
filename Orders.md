
# What can you do with Orders?

The nopCommerce API lets you do the following with the Customer resource.

+ [GET /api/orders  
Receive a list of all Orders](#get-apiorders)

+ [GET /api/orders/count
Retrieve a count of all orders](#get-apiorderscount)

+ [GET /api/orders/{id}  
Retrieve order by specified id](#get-apiordersid)

+ [GET /api/orders/customer/{customer_id}  
Retrieve orders for a specified customer ID](#get-apiorderscustomercustomerid)

+ [POST /api/orders  
Create an Order](#post-apiorders)

+ [DELETE /api/orders  
Delete an Order](#delete-apiordersid)

+ [PUT /api/orders  
Update an Order](#put-apiordersid)

# Customer Endpoints


## GET /api/orders  
Retrieve all orders


|  GET |  /api/orders |
|:---|:---|
|  ids |  A comma-separated list of order ids |
|  since_id |  Restrict results to after the specified ID |
|  limit |  Amount of results (default: 50) (maximum: 250) |
|  page |  Page to show (default: 1) |
|  fields |  Comma-separated list of fields to include in the response |


### GET /api/orders  
Get all orders

<details><summary>Response</summary><p>

```text

HTTP/1.1 200 OK

```

```json

{
    "orders": [
        {
            "id": "1300",
            "store_id": 1,
            "pick_up_in_store": true,
            "payment_method_system_name": "Payments.Stripe",
            "customer_currency_code": "USD",
            "currency_rate": 1,
            "customer_tax_display_type_id": 10,
            "vat_number": null,
            "order_subtotal_incl_tax": 85.6,
            "order_subtotal_excl_tax": 80,
            "order_sub_total_discount_incl_tax": 0,
            "order_sub_total_discount_excl_tax": 0,
            "order_shipping_incl_tax": 0,
            "order_shipping_excl_tax": 0,
            "payment_method_additional_fee_incl_tax": 0,
            "payment_method_additional_fee_excl_tax": 0,
            "tax_rates": "7.0000:5.60;   ",
            "order_tax": 5.6,
            "order_discount": 0,
            "order_total": 85.6,
            "refunded_amount": 0,
            "reward_points_were_added": null,
            "checkout_attribute_description": "",
            "customer_language_id": 1,
            "affiliate_id": 0,
            "customer_ip": "71.67.123.17",
            "authorization_transaction_id": null,
            "authorization_transaction_code": null,
            "authorization_transaction_result": null,
            "capture_transaction_id": "aaaaaaaaaaaaaaaaa",
            "capture_transaction_result": "succeeded",
            "subscription_transaction_id": null,
            "paid_date_utc": "2018-01-01T12:45:00.000",
            "shipping_method": "Pickup at Store",
            "shipping_rate_computation_method_system_name": "Pickup.PickupInStore",
            "custom_values_xml": "...",
            "deleted": false,
            "created_on_utc": "2018-01-01T12:45:00.000",
            "customer": {
                "id": "987654",
                "username": null,
                "email": null,
                "first_name": null,
                "last_name": null,
                "language_id": null,
                "date_of_birth": null,
                "gender": null,
                "admin_comment": null,
                "is_tax_exempt": false,
                "has_shopping_cart_items": false,
                "active": true,
                "deleted": false,
                "is_system_account": false,
                "system_name": null,
                "last_ip_address": "10.10.10.10",
                "created_on_utc": "2018-01-01T12:45:00.000",
                "last_login_date_utc": null,
                "last_activity_date_utc": "2018-01-01T12:45:00.000",
                "registered_in_store_id": 0,
                "subscribed_to_newsletter": false,
                "role_ids": [
                    4
                ]
            },
            "customer_id": 987654,
            "billing_address": {
                "id": "12345",
                "first_name": "Joe",
                "last_name": "Customer",
                "email": "joe@customer.com",
                "company": null,
                "country_id": 1,
                "country": "United States",
                "state_province_id": 44,
                "city": "Cincinnati",
                "address1": "100 Pine St",
                "address2": null,
                "zip_postal_code": "45000",
                "phone_number": "513-555-0123",
                "fax_number": null,
                "customer_attributes": "",
                "created_on_utc": "2018-01-01T12:45:00.000",
                "province": "Ohio"
            },
            "shipping_address": null,
            "order_items": [
                {
                    "id": "16405",
                    "product_attributes": [
                        {
                            "id": 2088,
                            "value": "8099"
                        },
                        {
                            "id": 2141,
                            "value": "8257"
                        }
                    ],
                    "quantity": 1,
                    "unit_price_incl_tax": 51.36,
                    "unit_price_excl_tax": 48,
                    "price_incl_tax": 51.36,
                    "price_excl_tax": 48,
                    "discount_amount_incl_tax": 0,
                    "discount_amount_excl_tax": 0,
                    "original_product_cost": 0,
                    "attribute_description": "Size: Adult XL<br />Personalized: No",
                    "download_count": 0,
                    "isDownload_activated": false,
                    "license_download_id": 0,
                    "item_weight": 1.1,
                    "rental_start_date_utc": null,
                    "rental_end_date_utc": null,
                    "product": {
                        "id": "818",
                        "visible_individually": true,
                        "name": "Men's Jacket",
                        "localized_names": [
                            {
                                "language_id": 1,
                                "localized_name": "Men's Jacket"
                            }
                        ],
                        "short_description": "Long description here",
                        "full_description": null,
                        "show_on_home_page": false,
                        "meta_keywords": null,
                        "meta_description": null,
                        "meta_title": null,
                        "allow_customer_reviews": false,
                        "approved_rating_sum": 0,
                        "not_approved_rating_sum": 0,
                        "approved_total_reviews": 0,
                        "not_approved_total_reviews": 0,
                        "sku": "ST970",
                        "manufacturer_part_number": null,
                        "gtin": null,
                        "is_gift_card": false,
                        "require_other_products": false,
                        "automatically_add_required_products": false,
                        "is_download": false,
                        "unlimited_downloads": true,
                        "max_number_of_downloads": 10,
                        "download_expiration_days": null,
                        "has_sample_download": false,
                        "has_user_agreement": false,
                        "is_recurring": false,
                        "recurring_cycle_length": 100,
                        "recurring_total_cycles": 10,
                        "is_rental": false,
                        "rental_price_length": 1,
                        "is_ship_enabled": true,
                        "is_free_shipping": false,
                        "ship_separately": false,
                        "additional_shipping_charge": 0,
                        "is_tax_exempt": false,
                        "is_telecommunications_or_broadcasting_or_electronic_services": false,
                        "use_multiple_warehouses": false,
                        "manage_inventory_method_id": 0,
                        "stock_quantity": 10000,
                        "display_stock_availability": false,
                        "display_stock_quantity": false,
                        "min_stock_quantity": 0,
                        "notify_admin_for_quantity_below": 1,
                        "allow_back_in_stock_subscriptions": false,
                        "order_minimum_quantity": 1,
                        "order_maximum_quantity": 10000,
                        "allowed_quantities": null,
                        "allow_adding_only_existing_attribute_combinations": false,
                        "disable_buy_button": false,
                        "disable_wishlist_button": false,
                        "available_for_pre_order": false,
                        "pre_order_availability_start_date_time_utc": null,
                        "call_for_price": false,
                        "price": 48,
                        "old_price": 0,
                        "product_cost": 0,
                        "special_price": null,
                        "special_price_start_date_time_utc": null,
                        "special_price_end_date_time_utc": null,
                        "customer_enters_price": false,
                        "minimum_customer_entered_price": 0,
                        "maximum_customer_entered_price": 1000,
                        "baseprice_enabled": false,
                        "baseprice_amount": 0,
                        "baseprice_base_amount": 0,
                        "has_tier_prices": false,
                        "has_discounts_applied": false,
                        "weight": 1.1,
                        "length": 0,
                        "width": 0,
                        "height": 0,
                        "available_start_date_time_utc": null,
                        "available_end_date_time_utc": null,
                        "display_order": 0,
                        "published": true,
                        "deleted": false,
                        "created_on_utc": "2018-01-01T12:45:00.000",
                        "updated_on_utc": "2018-01-01T12:45:00.000",
                        "product_type": "SimpleProduct",
                        "parent_grouped_product_id": 0,
                        "role_ids": [],
                        "discount_ids": [],
                        "store_ids": [
                            1
                        ],
                        "manufacturer_ids": [],
                        "images": [
                            {
                                "id": 999,
                                "position": 0,
                                "src": "https://example.com/image1.png",
                                "attachment": null
                            },
                            {
                                "id": 998,
                                "position": 0,
                                "src": "https://example.com/image2.png",
                                "attachment": null
                            }
                        ],
                        "attributes": [
                            {
                                "id": 2088,
                                "product_attribute_id": 2,
                                "product_attribute_name": "Size",
                                "text_prompt": null,
                                "is_required": true,
                                "attribute_control_type_id": 2,
                                "display_order": 0,
                                "default_value": null,
                                "attribute_control_type_name": "RadioList",
                                "attribute_values": [
                                    {
                                        "id": 8097,
                                        "type_id": 0,
                                        "associated_product_id": 0,
                                        "name": "Adult M",
                                        "color_squares_rgb": null,
                                        "image_squares_image": null,
                                        "price_adjustment": 0,
                                        "weight_adjustment": 0,
                                        "cost": 0,
                                        "quantity": 0,
                                        "is_pre_selected": false,
                                        "display_order": 11,
                                        "product_image_id": null,
                                        "type": "Simple"
                                    },
                                    {
                                        "id": 8098,
                                        "type_id": 0,
                                        "associated_product_id": 0,
                                        "name": "Adult L",
                                        "color_squares_rgb": null,
                                        "image_squares_image": null,
                                        "price_adjustment": 0,
                                        "weight_adjustment": 0,
                                        "cost": 0,
                                        "quantity": 0,
                                        "is_pre_selected": false,
                                        "display_order": 12,
                                        "product_image_id": null,
                                        "type": "Simple"
                                    }
                                ]
                            }
                        ],
                        "associated_product_ids": [],
                        "tags": [],
                        "vendor_id": 0,
                        "se_name": "mens-jacket"
                    },
                    "product_id": 818
                }
            ],
            "order_status": "Processing",
            "payment_status": "Paid",
            "shipping_status": "NotYetShipped",
            "customer_tax_display_type": "ExcludingTax"
        }
    ]
}

```

</p></details>

## GET /api/orders/count

Retrieve a count of all orders

|  GET |  /api/orders/id |

```text

HTTP/1.1 200 OK

```

```json

{
    "count": 12102
}

```

## GET /api/orders/{id}

Retrieve order by specified id

|  GET |  /api/orders/{id} |
|:---|:---|
|  fields |  Comma-separated list of fields to include in the response |

## GET /api/orders/customer/{customer_id}

Retrieve orders for a specified customer ID

## POST /api/orders

Create an Order

## DELETE /api/orders/{id}

Delete a specified Order

## PUT /api/orders/{id}

Update an Order

