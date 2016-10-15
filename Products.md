

# What can you do with Products?

The nopCommerce API lets you do the following with the Product resource.

+ [GET /api/products?fields=id,name,images  
Receive a list of all Products](#get-apiproducts)

+ [GET /api/products/count  
Receive a count of all Products](#get-apiproductscount)

+ [GET /api/products/{id}?fields=id,name,images  
Receive a single Product](#get-apiproductsid)

+ [POST /api/products  
Create a new Product](#post-apiproducts)

+ [PUT /api/products/{id}  
Modify an existing Product](#put-apiproductsid)

+ [DELETE /api/products/{id}  
Remove a Product from the database (mark as Deleted)](#delete-apiproductsid)

# Product Endpoints


## GET /api/products
Retrieve all customers


|  GET |  /api/products |
|:---|:---|
|  ids  |  A comma-separated list of order ids |
|  limit |  Amount of results (default: 50) (maximum: 250) |
|  page |  Page to show (default: 1) |
|  since_id |  Restrict results to after the specified ID |
|  published_status | <ul><li><strong>published</strong> - Show only published products</li><br/><li><strong>unpublished</strong> - Show only unpublished products</li><br/><li><strong>any</strong> - Show all products (<strong>default</strong>)</li></ul> |
|  vendor_name | Filter by product vendor |
|  category_id | Show only the products mapped to the specified category  |
|  created_at_min |  Show products created after date (format: 2014-04-25T16:15:47-04:00) |
|  created_at_max |  Show products created before date (format: 2014-04-25T16:15:47-04:00) |
|  updated_at_min |  Show products last updated after date (format: 2014-04-25T16:15:47-04:00) |
|  updated_at_max |  Show products last updated before date (format: 2014-04-25T16:15:47-04:00) |
|  fields |  Comma-separated list of fields to include in the response |


### GET /api/products?fields=id,name,images
Get all products, showing only some attributes

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "products": [
    {
      "id": "1",
      "name": "Build your own computer",
      "images": [
        {
          "src": null,
          "attachment":"/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDA"
          },
        {
          "src": null,
           "attachment":"/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDA"
           }
      ]
    },
    {
      "id": "2",
      "name": "Digital Storm VANQUISH 3 Custom Performance PC",
      "images": [
        {
          "src": null,
          "attachment":"/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDA"
          }
      ]
    }
  ]
}
```
</p></details>


### GET /api/products?ids=1,2
Get a list of specific products

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "products": [
    {
      "id": "1",
      "visible_individually": true,
      "name": "Build your own computer",
      "short_description": "Build it",
      "full_description": "&lt;p&gt;Fight back against cluttered workspaces with the stylish IBM zBC12 All-in-One desktop PC, featuring powerful computing resources and a stunning 20.1-inch widescreen display with stunning XBRITE-HiColor LCD technology. The black IBM zBC12 has a built-in microphone and MOTION EYE camera with face-tracking technology that allows for easy communication with friends and family. And it has a built-in DVD burner and Sony&#39;s Movie Store software so you can create a digital entertainment library for personal viewing at your convenience. Easy to setup and even easier to use, this JS-series All-in-One includes an elegantly designed keyboard and a USB mouse.&lt;/p&gt;",
      "show_on_home_page": true,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 0,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 0,
      "not_approved_total_reviews": 0,
      "sku": "COMP_CUST",
      "manufacturer_part_number": null,
      "gtin": null,
      "is_gift_card": false,
      "require_other_products": false,
      "automatically_add_required_products": false,
      "is_download": false,
      "unlimited_downloads": false,
      "max_number_of_downloads": 0,
      "download_expiration_days": null,
      "has_sample_download": false,
      "has_user_agreement": false,
      "is_recurring": false,
      "recurring_cycle_length": 0,
      "recurring_total_cycles": 0,
      "is_rental": false,
      "rental_price_length": 0,
      "is_ship_enabled": true,
      "is_free_shipping": true,
      "ship_separately": false,
      "additional_shipping_charge": 0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
      "stock_quantity": 10000,
      "display_stock_availability": true,
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
      "price": 1200,
      "old_price": 0,
      "product_cost": 0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0,
      "maximum_customer_entered_price": 0,
      "baseprice_enabled": false,
      "baseprice_amount": 0,
      "baseprice_base_amount": 0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 2,
      "length": 2,
      "width": 2,
      "height": 2,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-09-30T08:56:19.107",
      "updated_on_utc": "2016-09-30T08:56:19.107",
      "product_type": "SimpleProduct",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [
        {
          "src": null,
          "attachment": "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAcFBQYFBAcGBQY"
        },
        {
          "src": null,
          "attachment": "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAcFBQYFBAcGBQYI"
        }
      ],
      "tags": [
        "computer",
        "awesome"
      ],
      "vendor_id": 0,
      "se_name": null
    },
    {
      "id": "2",
      "visible_individually": true,
      "name": "Digital Storm VANQUISH 3 Custom Performance PC",
      "short_description": "Digital Storm Vanquish 3 Desktop PC",
      "full_description": "&lt;p&gt;Blow the doors off today’s most demanding games with maximum detail, speed, and power for an immersive gaming experience without breaking the bank.&lt;/p&gt;&lt;p&gt;Stay ahead of the competition, VANQUISH 3 is fully equipped to easily handle future upgrades, keeping your system on the cutting edge for years to come.&lt;/p&gt;&lt;p&gt;Each system is put through an extensive stress test, ensuring you experience zero bottlenecks and get the maximum performance from your hardware.&lt;/p&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 4,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 1,
      "not_approved_total_reviews": 0,
      "sku": "DS_VA3_PC",
      "manufacturer_part_number": null,
      "gtin": null,
      "is_gift_card": false,
      "require_other_products": false,
      "automatically_add_required_products": false,
      "is_download": false,
      "unlimited_downloads": false,
      "max_number_of_downloads": 0,
      "download_expiration_days": null,
      "has_sample_download": false,
      "has_user_agreement": false,
      "is_recurring": false,
      "recurring_cycle_length": 0,
      "recurring_total_cycles": 0,
      "is_rental": false,
      "rental_price_length": 0,
      "is_ship_enabled": true,
      "is_free_shipping": false,
      "ship_separately": false,
      "additional_shipping_charge": 0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
      "stock_quantity": 10000,
      "display_stock_availability": true,
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
      "price": 1259,
      "old_price": 0,
      "product_cost": 0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0,
      "maximum_customer_entered_price": 0,
      "baseprice_enabled": false,
      "baseprice_amount": 0,
      "baseprice_base_amount": 0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 7,
      "length": 7,
      "width": 7,
      "height": 7,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-09-30T08:56:19.43",
      "updated_on_utc": "2016-09-30T08:56:19.43",
      "product_type": "SimpleProduct",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [
        {
          "src": null,
          "attachment": "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAcFBQYFB"
        }
      ],
      "tags": [
        "cool",
        "computer"
      ],
      "vendor_id": 0,
      "se_name": null
    }
  ]
}
```
</p></details>

### GET /api/products?since_id=1
Get all products after the specified ID

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
 
{
  "products": [    
    {
      "id": "2",
      "visible_individually": true,
      "name": "Digital Storm VANQUISH 3 Custom Performance PC",
      "short_description": "Digital Storm Vanquish 3 Desktop PC",
      "full_description": "&lt;p&gt;Blow the doors off today’s most demanding games with maximum detail, speed, and power for an immersive gaming experience without breaking the bank.&lt;/p&gt;&lt;p&gt;Stay ahead of the competition, VANQUISH 3 is fully equipped to easily handle future upgrades, keeping your system on the cutting edge for years to come.&lt;/p&gt;&lt;p&gt;Each system is put through an extensive stress test, ensuring you experience zero bottlenecks and get the maximum performance from your hardware.&lt;/p&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 4,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 1,
      "not_approved_total_reviews": 0,
      "sku": "DS_VA3_PC",
      "manufacturer_part_number": null,
      "gtin": null,
      "is_gift_card": false,
      "require_other_products": false,
      "automatically_add_required_products": false,
      "is_download": false,
      "unlimited_downloads": false,
      "max_number_of_downloads": 0,
      "download_expiration_days": null,
      "has_sample_download": false,
      "has_user_agreement": false,
      "is_recurring": false,
      "recurring_cycle_length": 0,
      "recurring_total_cycles": 0,
      "is_rental": false,
      "rental_price_length": 0,
      "is_ship_enabled": true,
      "is_free_shipping": false,
      "ship_separately": false,
      "additional_shipping_charge": 0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
      "stock_quantity": 10000,
      "display_stock_availability": true,
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
      "price": 1259,
      "old_price": 0,
      "product_cost": 0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0,
      "maximum_customer_entered_price": 0,
      "baseprice_enabled": false,
      "baseprice_amount": 0,
      "baseprice_base_amount": 0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 7,
      "length": 7,
      "width": 7,
      "height": 7,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-09-30T08:56:19.43",
      "updated_on_utc": "2016-09-30T08:56:19.43",
      "product_type": "SimpleProduct",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [
        {
          "src": null,
          "attachment": "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAcFBQYFB"
        }
      ],
      "tags": [
        "cool",
        "computer"
      ],
      "vendor_id": 0,
      "se_name": null
    }
  ]
}       
```
</p></details>



### GET /api/products?category_id=3&fields=id,name
Fetches all products that belong to a certain category

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK

{
  "products": [
    {
      "id": "4",
      "name": "Apple MacBook Pro 13-inch"
    },
    {
      "id": "5",
      "name": "Asus N551JK-XO076H Laptop"
    },
    {
      "id": "6",
      "name": "Samsung Series 9 NP900X4C Premium Ultrabook"
    },
    {
      "id": "7",
      "name": "HP Spectre XT Pro UltraBook"
    },
    {
      "id": "8",
      "name": "HP Envy 6-1180ca 15.6-Inch Sleekbook"
    },
    {
      "id": "9",
      "name": "Lenovo Thinkpad X1 Carbon Laptop"
    }
  ]
}
```
</p></details>



## GET /api/products/count
Get a count of all products


|  GET |  /api/products/count |
|:---|:---|
|  published_status | <ul><li><strong>published</strong> - Show only published products</li><br/><br/><li><strong>unpublished</strong> - Show only unpublished products</li><br/><br/><li><strong>any</strong> - Show all products (<strong>default</strong>)</li></ul> |
|  vendor_name | Filter by product vendor |
|  category_id | Show only the products mapped to the specified category  |
|  created_at_min |  Show products created after date (format: 2014-04-25T16:15:47-04:00) |
|  created_at_max |  Show products created before date (format: 2014-04-25T16:15:47-04:00) |
|  updated_at_min |  Show products last updated after date (format: 2014-04-25T16:15:47-04:00) |
|  updated_at_max |  Show products last updated before date (format: 2014-04-25T16:15:47-04:00) |


### GET /api/products/count  
Count all products

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "count": 45
}
```
</p></details>



### GET /api/products/count?category_id=3  
Count all products belonging to a certain category

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "count": 6
}
```
</p></details>



## GET /api/products/{id}  
Get a single product


|  GET |  /api/products/{id} |
|:---|:---|
|  fields |  Comma-separated list of fields to include in the response |



### GET /api/products/2  
Get a single product by id

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "products": [    
    {
      "id": "2",
      "visible_individually": true,
      "name": "Digital Storm VANQUISH 3 Custom Performance PC",
      "short_description": "Digital Storm Vanquish 3 Desktop PC",
      "full_description": "&lt;p&gt;Blow the doors off today’s most demanding games with maximum detail, speed, and power for an immersive gaming experience without breaking the bank.&lt;/p&gt;&lt;p&gt;Stay ahead of the competition, VANQUISH 3 is fully equipped to easily handle future upgrades, keeping your system on the cutting edge for years to come.&lt;/p&gt;&lt;p&gt;Each system is put through an extensive stress test, ensuring you experience zero bottlenecks and get the maximum performance from your hardware.&lt;/p&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 4,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 1,
      "not_approved_total_reviews": 0,
      "sku": "DS_VA3_PC",
      "manufacturer_part_number": null,
      "gtin": null,
      "is_gift_card": false,
      "require_other_products": false,
      "automatically_add_required_products": false,
      "is_download": false,
      "unlimited_downloads": false,
      "max_number_of_downloads": 0,
      "download_expiration_days": null,
      "has_sample_download": false,
      "has_user_agreement": false,
      "is_recurring": false,
      "recurring_cycle_length": 0,
      "recurring_total_cycles": 0,
      "is_rental": false,
      "rental_price_length": 0,
      "is_ship_enabled": true,
      "is_free_shipping": false,
      "ship_separately": false,
      "additional_shipping_charge": 0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
      "stock_quantity": 10000,
      "display_stock_availability": true,
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
      "price": 1259,
      "old_price": 0,
      "product_cost": 0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0,
      "maximum_customer_entered_price": 0,
      "baseprice_enabled": false,
      "baseprice_amount": 0,
      "baseprice_base_amount": 0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 7,
      "length": 7,
      "width": 7,
      "height": 7,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-09-30T08:56:19.43",
      "updated_on_utc": "2016-09-30T08:56:19.43",
      "product_type": "SimpleProduct",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [
        {
          "src": null,
          "attachment": "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAcFBQYFB"
        }
      ],
      "tags": [
        "cool",
        "computer"
      ],
      "vendor_id": 0,
      "se_name": null
    }
  ]
}       
```
</p></details>



### GET /api/products/2?fields=id,name
Get a single product with id 2 and show only the id and the name in the response

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "products": [
    {
      "id": "2",
      "name": "Digital Storm VANQUISH 3 Custom Performance PC"
    }
  ]
}
```
</p></details>



## POST /api/products
Create a new product


### Trying to create a product without a name will return an error  
POST /api/products
```json
{
  "product": {
    "name": ""
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 422 Unprocessable Entity
         
{
  "errors": {
    "Name": [
      "'Name' must not be empty.",
      "name is required"
    ]
  }
}
```
</p></details>



### Create a new product record  
POST /api/product
```json
{
  "product": {
    "name": "Skate Banana",
    "full_description": "<strong>Great snowboard!</strong>",
    "short_description": "TWIN ALL TERRAIN FUN"
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK  
         
{
  "products": [
    {
      "id": "47",
      "visible_individually": true,
      "name": "Skate Banana",
      "short_description": "TWIN ALL TERRAIN FUN",
      "full_description": "&lt;strong&gt;Great snowboard!&lt;/strong&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 0,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 0,
      "not_approved_total_reviews": 0,
      "sku": null,
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
      "additional_shipping_charge": 0.0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
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
      "price": 0.0,
      "old_price": 0.0,
      "product_cost": 0.0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0.0,
      "maximum_customer_entered_price": 1000.0,
      "baseprice_enabled": false,
      "baseprice_amount": 0.0,
      "baseprice_base_amount": 0.0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 1.00000000,
      "length": 0.0,
      "width": 0.0,
      "height": 0.0,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-10-15T16:08:30.1363088Z",
      "updated_on_utc": "2016-10-15T16:08:30.1363088Z",
      "product_type": "0",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [],
      "tags": [],
      "vendor_id": 0,
      "se_name": "skate-banana"
    }
  ]
}
```
</p></details>



### Create a new product with a product image which will be downloaded by nopCommerce  
POST /api/products  
```json
{
  "product": {
    "name": "Skate Banana",
    "full_description": "<strong>Great snowboard!</strong>",
    "short_description": "TWIN ALL TERRAIN FUN",
    "images": [
      {
        "src": "http://cdn.lib-tech.com/wp-content/uploads/2016/07/2016-2017-Lib-Tech-Skate-Banana-Yellow-Snowboard-800x800.png"
      }
    ]
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK  
         
{
  "products": [
    {
      "id": "48",
      "visible_individually": true,
      "name": "Skate Banana",
      "short_description": "TWIN ALL TERRAIN FUN",
      "full_description": "&lt;strong&gt;Great snowboard!&lt;/strong&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 0,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 0,
      "not_approved_total_reviews": 0,
      "sku": null,
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
      "additional_shipping_charge": 0.0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
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
      "price": 0.0,
      "old_price": 0.0,
      "product_cost": 0.0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0.0,
      "maximum_customer_entered_price": 1000.0,
      "baseprice_enabled": false,
      "baseprice_amount": 0.0,
      "baseprice_base_amount": 0.0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 1.00000000,
      "length": 0.0,
      "width": 0.0,
      "height": 0.0,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-10-15T16:23:36.0206655Z",
      "updated_on_utc": "2016-10-15T16:23:36.0206655Z",
      "product_type": "0",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [
        {
          "src": null,
          "attachment": "iVBORw0KGgoAAAANSUhEUgAAAyAAAAMgCAYAAADbcAZoAA"          
        }
      ],
      "tags": [],
      "vendor_id": 0,
      "se_name": "skate-banana-2"
    }
  ]
}
```
</p></details>



### Create a new product with a base64 encoded image  
POST /api/products  
```json
{
  "product": {
    "name": "Skate Banana",
    "full_description": "<strong>Great snowboard!</strong>",
    "short_description": "TWIN ALL TERRAIN FUN",
    "images": [
      {
        "attachment": "iVBORw0KGgoAAAANSUhEUgAAAyAAAAMgCAYAAADbcAZoAA"
      }
    ]
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK  
         
{
  "products": [
    {
      "id": "49",
      "visible_individually": true,
      "name": "Skate Banana",
      "short_description": "TWIN ALL TERRAIN FUN",
      "full_description": "&lt;strong&gt;Great snowboard!&lt;/strong&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 0,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 0,
      "not_approved_total_reviews": 0,
      "sku": null,
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
      "additional_shipping_charge": 0.0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
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
      "price": 0.0,
      "old_price": 0.0,
      "product_cost": 0.0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0.0,
      "maximum_customer_entered_price": 1000.0,
      "baseprice_enabled": false,
      "baseprice_amount": 0.0,
      "baseprice_base_amount": 0.0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 1.00000000,
      "length": 0.0,
      "width": 0.0,
      "height": 0.0,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-10-15T16:23:36.0206655Z",
      "updated_on_utc": "2016-10-15T16:23:36.0206655Z",
      "product_type": "0",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [
        {
          "src": null,
          "attachment": "iVBORw0KGgoAAAANSUhEUgAAAyAAAAMgCAYAAADbcAZoAA"          
        }
      ],
      "tags": [],
      "vendor_id": 0,
      "se_name": "skate-banana-3"
    }
  ]
}
```
</p></details>



### Create a new, but unpublished product  
POST /api/products  
```json
{
  "product": {
    "name": "Skate Banana",
    "full_description": "<strong>Great snowboard!</strong>",
    "short_description": "TWIN ALL TERRAIN FUN",
    "published": false
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK  
         
{
  "products": [
    {
      "id": "50",
      "visible_individually": true,
      "name": "Skate Banana",
      "short_description": "TWIN ALL TERRAIN FUN",
      "full_description": "&lt;strong&gt;Great snowboard!&lt;/strong&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 0,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 0,
      "not_approved_total_reviews": 0,
      "sku": null,
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
      "additional_shipping_charge": 0.0,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
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
      "price": 0.0,
      "old_price": 0.0,
      "product_cost": 0.0,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0.0,
      "maximum_customer_entered_price": 1000.0,
      "baseprice_enabled": false,
      "baseprice_amount": 0.0,
      "baseprice_base_amount": 0.0,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 1.00000000,
      "length": 0.0,
      "width": 0.0,
      "height": 0.0,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": false,
      "deleted": false,
      "created_on_utc": "2016-10-15T17:00:31.5466951Z",
      "updated_on_utc": "2016-10-15T17:00:31.5466951Z",
      "product_type": "0",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [],
      "tags": [],
      "vendor_id": 0,
      "se_name": "skate-banana-4"
    }
  ]
}
```
</p></details>



## PUT /api/products/{id}  
Update a product


### Update a product's name and short description
POST /api/customers/49  
```json
{
  "product": {
    "name": "Updated Skate Banana",
    "short_description": "Updated TWIN ALL TERRAIN FUN"    
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "products": [
    {
      "id": "49",
      "visible_individually": true,
      "name": "Updated Skate Banana",
      "short_description": "Updated TWIN ALL TERRAIN FUN",
      "full_description": "&lt;strong&gt;Great snowboard!&lt;/strong&gt;",
      "show_on_home_page": false,
      "meta_keywords": null,
      "meta_description": null,
      "meta_title": null,
      "allow_customer_reviews": true,
      "approved_rating_sum": 0,
      "not_approved_rating_sum": 0,
      "approved_total_reviews": 0,
      "not_approved_total_reviews": 0,
      "sku": null,
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
      "additional_shipping_charge": 0.0000,
      "is_tax_exempt": false,
      "is_telecommunications_or_broadcasting_or_electronic_services": false,
      "use_multiple_warehouses": false,
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
      "price": 0.0000,
      "old_price": 0.0000,
      "product_cost": 0.0000,
      "special_price": null,
      "special_price_start_date_time_utc": null,
      "special_price_end_date_time_utc": null,
      "customer_enters_price": false,
      "minimum_customer_entered_price": 0.0000,
      "maximum_customer_entered_price": 1000.0000,
      "baseprice_enabled": false,
      "baseprice_amount": 0.0000,
      "baseprice_base_amount": 0.0000,
      "has_tier_prices": false,
      "has_discounts_applied": false,
      "weight": 1.0000,
      "length": 0.0000,
      "width": 0.0000,
      "height": 0.0000,
      "available_start_date_time_utc": null,
      "available_end_date_time_utc": null,
      "display_order": 0,
      "published": true,
      "deleted": false,
      "created_on_utc": "2016-10-15T16:44:20.427",
      "updated_on_utc": "2016-10-15T17:16:32.477802Z",
      "product_type": "0",
      "role_ids": [],
      "discount_ids": [],
      "store_ids": [],
      "manufacturer_ids": [],
      "images": [],
      "tags": [],
      "vendor_id": 0,
      "se_name": "updated-skate-banana"
    }
  ]
}
```
</p></details>



## DELETE /api/products/{id}  
Mark a product as Deleted.


### Remove an existing product (will be marked as Deleted)  
DELETE /api/products/50  

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{}
```
</p></details>




