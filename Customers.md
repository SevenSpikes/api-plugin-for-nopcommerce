
# What can you do with Customers?

The nopCommerce API lets you do the following with the Customer resource.

+ [GET /api/customers  
Receive a list of all Customers](#get-apicustomers)

+ [GET /api/customers/search?query=first_name:john  
Search for customers matching supplied query](#get-apicustomerssearch)

+ [GET /api/customers/{id}  
Receive a single Customer](#get-apicustomersid)

+ [POST /api/customers  
Create a new Customer](#post-apicustomers)

+ [PUT /api/customers/{id}  
Modify an existing Customer](#put-apicustomersid)

+ [DELETE /api/customers/{id}  
Remove a Customer (mark as Deleted)](#delete-apicustomersid)

+ [GET /api/customers/count  
Receive a count of all Customers](#get-apicustomerscount)

+ [GET /api/orders/customer/{customer_id}  
Find orders belonging to this customer](#get-apiorderscustomercustomer_id)

# Customer Endpoints


## GET /api/customers  
Retrieve all customers


|  GET |  /api/customers |
|:---|:---|
|  since_id |  Restrict results to after the specified ID |
|  created_at_min |  Show customers created after date (format: 2014-04-25T16:15:47-04:00) |
|  created_at_max |  Show customers created before date (format: 2014-04-25T16:15:47-04:00) |
|  limit |  Amount of results (default: 50) (maximum: 250) |
|  page |  Page to show (default: 1) |
|  fields |  Comma-separated list of fields to include in the response |


### GET /api/customers  
Get all customers

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "1",
        "first_name": "John",
        "last_name": "Smith",
        "email": "admin@yourStore.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": "",
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.85",
        "province": "New York"
      },
      "shipping_address": {
        "id": "1",
        "first_name": "John",
        "last_name": "Smith",
        "email": "admin@yourStore.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": "",
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.85",
        "province": "New York"
      },
      "addresses": [
        {
          "id": "1",
          "first_name": "John",
          "last_name": "Smith",
          "email": "admin@yourStore.com",
          "company": "Nop Solutions Ltd",
          "country_id": 1,
          "country": "United States",
          "state_province_id": 40,
          "city": "New York",
          "address1": "21 West 52nd Street",
          "address2": "",
          "zip_postal_code": "10021",
          "phone_number": "12345678",
          "fax_number": "",
          "customer_attributes": null,
          "created_on_utc": "2016-09-30T08:56:13.85",
          "province": "New York"
        }
      ],
      "id": "1",
      "username": "admin@yourStore.com",
      "email": "admin@yourStore.com",
      "first_name": "John",
      "last_name": "Smith",
      "admin_comment": null,
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": "127.0.0.1",
      "created_on_utc": "2016-09-30T08:56:13.443",
      "last_login_date_utc": "2016-10-12T19:59:05.063",
      "last_activity_date_utc": "2016-10-12T19:59:05.137",
      "role_ids": []
    }
  ]
}
```
</p></details>


### GET /api/customers?created_at_min=2016-09-30T08:56:13.85  
Get all customers created after a certain date

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "2",
        "first_name": "Steve",
        "last_name": "Gates",
        "email": "steve_gates@nopCommerce.com",
        "company": "Steve Company",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 9,
        "city": "Los Angeles",
        "address1": "750 Bel Air Rd.",
        "address2": "",
        "zip_postal_code": "90077",
        "phone_number": "87654321",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.97",
        "province": "California"
      },
      "shipping_address": {
        "id": "2",
        "first_name": "Steve",
        "last_name": "Gates",
        "email": "steve_gates@nopCommerce.com",
        "company": "Steve Company",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 9,
        "city": "Los Angeles",
        "address1": "750 Bel Air Rd.",
        "address2": "",
        "zip_postal_code": "90077",
        "phone_number": "87654321",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.97",
        "province": "California"
      },
      "addresses": [
        {
          "id": "2",
          "first_name": "Steve",
          "last_name": "Gates",
          "email": "steve_gates@nopCommerce.com",
          "company": "Steve Company",
          "country_id": 1,
          "country": "United States",
          "state_province_id": 9,
          "city": "Los Angeles",
          "address1": "750 Bel Air Rd.",
          "address2": "",
          "zip_postal_code": "90077",
          "phone_number": "87654321",
          "fax_number": "",
          "customer_attributes": null,
          "created_on_utc": "2016-09-30T08:56:13.97",
          "province": "California"
        }
      ],
      "id": "2",
      "username": "steve_gates@nopCommerce.com",
      "email": "steve_gates@nopCommerce.com",
      "first_name": "Steve",
      "last_name": "Gates",
      "admin_comment": null,
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": null,
      "created_on_utc": "2016-09-30T08:56:13.967",
      "last_login_date_utc": null,
      "last_activity_date_utc": "2016-09-30T08:56:13.967",
      "role_ids": []
    }
  ]
}
```
</p></details>



## GET /api/customers/search  
Search for customers matching supplied query


|  GET |  /api/customers/search |
|:---|:---|
|  order |  Field and direction to order results by (default: id DESC) |
|  query |  Text to search customers |
|  limit |  Amount of results (default: 50) (maximum: 250) |
|  page |  Page to show (default: 1) |
|  fields |  Comma-separated list of fields to include in the response |


### GET /api/customers/search?query=first_name:john  
Get all customers with first name "John"

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "1",
        "first_name": "John",
        "last_name": "Smith",
        "email": "admin@yourStore.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": "",
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.85",
        "province": "New York"
      },
      "shipping_address": {
        "id": "1",
        "first_name": "John",
        "last_name": "Smith",
        "email": "admin@yourStore.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": "",
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.85",
        "province": "New York"
      },
      "addresses": [
        {
          "id": "1",
          "first_name": "John",
          "last_name": "Smith",
          "email": "admin@yourStore.com",
          "company": "Nop Solutions Ltd",
          "country_id": 1,
          "country": "United States",
          "state_province_id": 40,
          "city": "New York",
          "address1": "21 West 52nd Street",
          "address2": "",
          "zip_postal_code": "10021",
          "phone_number": "12345678",
          "fax_number": "",
          "customer_attributes": null,
          "created_on_utc": "2016-09-30T08:56:13.85",
          "province": "New York"
        }
      ],
      "id": "1",
      "username": "admin@yourStore.com",
      "email": "admin@yourStore.com",
      "first_name": "John",
      "last_name": "Smith",
      "admin_comment": null,
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": "127.0.0.1",
      "created_on_utc": "2016-09-30T08:56:13.443",
      "last_login_date_utc": "2016-10-12T19:59:05.063",
      "last_activity_date_utc": "2016-10-12T19:59:05.137",
      "role_ids": []
    }
  ]
}
```
</p></details>



## GET /api/customers/{id}  
Retrieve customer by specified id


|  GET |  /api/customers/{id} |
|:---|:---|
|  fields |  Comma-separated list of fields to include in the response |


### GET /api/customers/1  
Get a single customer with id 1

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "1",
        "first_name": "John",
        "last_name": "Smith",
        "email": "admin@yourStore.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": "",
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.85",
        "province": "New York"
      },
      "shipping_address": {
        "id": "1",
        "first_name": "John",
        "last_name": "Smith",
        "email": "admin@yourStore.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": "",
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": "",
        "customer_attributes": null,
        "created_on_utc": "2016-09-30T08:56:13.85",
        "province": "New York"
      },
      "addresses": [
        {
          "id": "1",
          "first_name": "John",
          "last_name": "Smith",
          "email": "admin@yourStore.com",
          "company": "Nop Solutions Ltd",
          "country_id": 1,
          "country": "United States",
          "state_province_id": 40,
          "city": "New York",
          "address1": "21 West 52nd Street",
          "address2": "",
          "zip_postal_code": "10021",
          "phone_number": "12345678",
          "fax_number": "",
          "customer_attributes": null,
          "created_on_utc": "2016-09-30T08:56:13.85",
          "province": "New York"
        }
      ],
      "id": "1",
      "username": "admin@yourStore.com",
      "email": "admin@yourStore.com",
      "first_name": "John",
      "last_name": "Smith",
      "admin_comment": null,
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": "127.0.0.1",
      "created_on_utc": "2016-09-30T08:56:13.443",
      "last_login_date_utc": "2016-10-12T19:59:05.063",
      "last_activity_date_utc": "2016-10-12T19:59:05.137",
      "role_ids": []
    }
  ]
}
```
</p></details>



### GET /api/customers/1?fields=id,email  
Get a single customer with id 1 and add only the id and the email in the response

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "customers": [
    {
      "id": "1",
      "email": "admin@yourStore.com"
    }
  ]
}
```
</p></details>



## POST /api/customers  
Creates a new customer


### Trying to create a customer without an email or customer role will return an error  
POST /api/customers  
```json
{
  "customer": {
    "email": null,
    "role_ids": [],
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 422 Unprocessable Entity
         
{
  "errors": {
    "RoleIds": [
      "role_ids required"
    ],
    "Email": [
      "'Email' must not be empty.",
      "email can not be empty"
    ]
  }
}
```
</p></details>



### Create a new customer record  
POST /api/customers  
```json
{
  "customer": {
    "first_name": "Steve",
    "last_name": "Gates",
    "email": "steve.gates@example.com",
    "role_ids": [ 3 ]   
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK  
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "0",
        "first_name": null,
        "last_name": null,
        "email": null,
        "company": null,
        "country_id": null,
        "country": null,
        "state_province_id": null,
        "city": null,
        "address1": null,
        "address2": null,
        "zip_postal_code": null,
        "phone_number": null,
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "0001-01-01T00:00:00",
        "province": null
      },
      "shipping_address": {
        "id": "0",
        "first_name": null,
        "last_name": null,
        "email": null,
        "company": null,
        "country_id": null,
        "country": null,
        "state_province_id": null,
        "city": null,
        "address1": null,
        "address2": null,
        "zip_postal_code": null,
        "phone_number": null,
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "0001-01-01T00:00:00",
        "province": null
      },
      "addresses": [],
      "id": "85",
      "username": null,
      "email": "steve.gates@example.com",
      "first_name": "Steve",
      "last_name": "Gates",
      "admin_comment": null,
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": null,
      "created_on_utc": "2016-10-13T10:36:46.1537491Z",
      "last_login_date_utc": null,
      "last_activity_date_utc": "2016-10-13T10:36:46.1537491Z",
      "role_ids": [
        3
      ]
    }
  ]
}
```
</p></details>



### Create a new customer record with a billing address  
POST /api/customers  
```json
{
  "customer": {
    "first_name": "Steve",
    "last_name": "Gates",
    "email": "steve.gates@example.com",
    "role_ids": [
      3
    ],
    "billing_address": {
      "first_name": "Steve",
      "last_name": "Gates",
      "email": "steve.gates@example.com",
      "company": "Nop Solutions Ltd",
      "country_id": 1,
      "state_province_id": 40,
      "city": "New York",
      "address1": "21 West 52nd Street",
      "phone_number": "12345678",
      "zip_postal_code": "10021"
    }
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK  
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "25",
        "first_name": "Steve",
        "last_name": "Gates",
        "email": "steve.gates@example.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": null,
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "2016-10-13T11:18:07.7097928Z",
        "province": null
      },
      "shipping_address": {
        "id": "0",
        "first_name": null,
        "last_name": null,
        "email": null,
        "company": null,
        "country_id": null,
        "country": null,
        "state_province_id": null,
        "city": null,
        "address1": null,
        "address2": null,
        "zip_postal_code": null,
        "phone_number": null,
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "0001-01-01T00:00:00",
        "province": null
      },
      "addresses": [],
      "id": "97",
      "username": null,
      "email": "steve.gates@example.com",
      "first_name": "Steve",
      "last_name": "Gates",
      "admin_comment": null,
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": null,
      "created_on_utc": "2016-10-13T11:18:07.7097928Z",
      "last_login_date_utc": null,
      "last_activity_date_utc": "2016-10-13T11:18:07.7097928Z",
      "role_ids": [
        3
      ]
    }
  ]
}
```
</p></details>



## PUT /api/customers/{id}  
Update an existing customer


### Add shipping address to an existing customer  
PUT /api/customers/97  
```json
{
  "customer": {
    "shipping_address": {
      "first_name": "Steve",
      "last_name": "Gates",
      "email": "steve.gates@example.com",
      "company": "Nop Solutions Ltd",
      "country_id": 1,
      "state_province_id": 40,
      "city": "New York",
      "address1": "21 West 52nd Street",
      "phone_number": "12345678",
      "zip_postal_code": "10021"
    }
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "25",
        "first_name": "Steve",
        "last_name": "Gates",
        "email": "steve.gates@example.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": null,
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "2016-10-13T11:18:07.71",
        "province": "New York"
      },
      "shipping_address": {
        "id": "26",
        "first_name": "Steve",
        "last_name": "Gates",
        "email": "steve.gates@example.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": null,
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "2016-10-13T12:51:56.207",
        "province": "New York"
      },
      "addresses": [],
      "id": "97",
      "username": null,
      "email": "steve.gates@example.com",
      "first_name": "Steve",
      "last_name": "Gates",
      "admin_comment": null,
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": null,
      "created_on_utc": "2016-10-13T11:18:07.71",
      "last_login_date_utc": null,
      "last_activity_date_utc": "2016-10-13T11:18:07.71",
      "role_ids": [
        3
      ]
    }
  ]
}
```
</p></details>



### Update details for a customer  
PUT /api/customers/97  
```json
{
  "customer": {
    "admin_comment": "Customer is a great guy"
  }
}
```

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK  
         
{
  "customers": [
    {
      "shopping_cart_items": [],
      "billing_address": {
        "id": "25",
        "first_name": "Steve",
        "last_name": "Gates",
        "email": "steve.gates@example.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": null,
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "2016-10-13T11:18:07.71",
        "province": "New York"
      },
      "shipping_address": {
        "id": "26",
        "first_name": "Steve",
        "last_name": "Gates",
        "email": "steve.gates@example.com",
        "company": "Nop Solutions Ltd",
        "country_id": 1,
        "country": "United States",
        "state_province_id": 40,
        "city": "New York",
        "address1": "21 West 52nd Street",
        "address2": null,
        "zip_postal_code": "10021",
        "phone_number": "12345678",
        "fax_number": null,
        "customer_attributes": null,
        "created_on_utc": "2016-10-13T12:51:56.207",
        "province": "New York"
      },
      "addresses": [],
      "id": "97",
      "username": null,
      "email": "steve.gates@example.com",
      "first_name": "Steve",
      "last_name": "Gates",
      "admin_comment": "Customer is a great guy",
      "is_tax_exempt": false,
      "has_shopping_cart_items": false,
      "active": true,
      "deleted": false,
      "is_system_account": false,
      "system_name": null,
      "last_ip_address": null,
      "created_on_utc": "2016-10-13T11:18:07.71",
      "last_login_date_utc": null,
      "last_activity_date_utc": "2016-10-13T11:18:07.71",
      "role_ids": [
        3
      ]
    }
  ]
}
```
</p></details>



## DELETE /api/customers/{id}  
Mark a customer as Deleted. Deleted customers are not returned by the GET endpoints.


### Remove an existing customer (will be marked as Deleted)  
DELETE /api/customers/97  

<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{}
```
</p></details>




## GET /api/customers/count  
Get a count of all customers

GET /api/customers/count
<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "count": 17
}
```
</p></details>




## GET /api/orders/customer/{customer_id}  
Get all orders belonging to this customer


GET /api/orders/customer/6
<details><summary>Response</summary><p>
```json
         HTTP/1.1 200 OK
         
{
  "orders": [
    {
      "id": "5",
      "store_id": 1,
      "pick_up_in_store": false,
      "payment_method_system_name": "Payments.CheckMoneyOrder",
      "customer_currency_code": "USD",
      "currency_rate": 1,
      "customer_tax_display_type_id": 0,
      "vat_number": "",
      "order_subtotal_incl_tax": 43.5,
      "order_subtotal_excl_tax": 43.5,
      "order_sub_total_discount_incl_tax": 0,
      "order_sub_total_discount_excl_tax": 0,
      "order_shipping_incl_tax": 0,
      "order_shipping_excl_tax": 0,
      "payment_method_additional_fee_incl_tax": 0,
      "payment_method_additional_fee_excl_tax": 0,
      "tax_rates": "0:0;",
      "order_tax": 0,
      "order_discount": 0,
      "order_total": 43.5,
      "refunded_amount": 0,
      "reward_points_were_added": false,
      "checkout_attribute_description": "",
      "customer_language_id": 1,
      "affiliate_id": 0,
      "customer_ip": "127.0.0.1",
      "authorization_transaction_id": "",
      "authorization_transaction_code": "",
      "authorization_transaction_result": "",
      "capture_transaction_id": "",
      "capture_transaction_result": "",
      "subscription_transaction_id": "",
      "paid_date_utc": "2016-09-30T08:56:28.033",
      "shipping_method": "Ground",
      "shipping_rate_computation_method_system_name": "Shipping.FixedRate",
      "custom_values_xml": "",
      "deleted": false,
      "created_on_utc": "2016-09-30T08:56:28.033",
      "customer": {
        "id": "6",
        "username": "victoria_victoria@nopCommerce.com",
        "email": "victoria_victoria@nopCommerce.com",
        "is_tax_exempt": false,
        "has_shopping_cart_items": false,
        "active": true,
        "deleted": false,
        "is_system_account": false,
        "created_on_utc": "2016-09-30T08:56:14.05",
        "last_activity_date_utc": "2016-09-30T08:56:14.05",
        "role_ids": []
      },
      "customer_id": 6,
      "billing_address": {
        "id": "18",
        "first_name": "Victoria",
        "last_name": "Terces",
        "email": "victoria_victoria@nopCommerce.com",
        "company": "Terces Company",
        "country_id": 2,
        "country": "Canada",
        "state_province_id": 74,
        "city": "Saskatoon",
        "address1": "201 1st Avenue South",
        "address2": "",
        "zip_postal_code": "S7K 1J9",
        "phone_number": "45612378",
        "fax_number": "",
        "created_on_utc": "2016-09-30T08:56:14.057",
        "province": "Saskatchewan"
      },
      "shipping_address": {
        "id": "19",
        "first_name": "Victoria",
        "last_name": "Terces",
        "email": "victoria_victoria@nopCommerce.com",
        "company": "Terces Company",
        "country_id": 2,
        "country": "Canada",
        "state_province_id": 74,
        "city": "Saskatoon",
        "address1": "201 1st Avenue South",
        "address2": "",
        "zip_postal_code": "S7K 1J9",
        "phone_number": "45612378",
        "fax_number": "",
        "created_on_utc": "2016-09-30T08:56:14.057",
        "province": "Saskatchewan"
      },
      "order_items": [
        {
          "quantity": 1,
          "unit_price_incl_tax": 43.5,
          "unit_price_excl_tax": 43.5,
          "price_incl_tax": 43.5,
          "price_excl_tax": 43.5,
          "discount_amount_incl_tax": 0,
          "discount_amount_excl_tax": 0,
          "original_product_cost": 0,
          "attribute_description": "",
          "download_count": 0,
          "isDownload_activated": false,
          "license_download_id": 0,
          "product": {
            "id": "30",
            "visible_individually": true,
            "name": "Levi's 511 Jeans",
            "short_description": "Levi's Faded Black 511 Jeans ",
            "full_description": "&lt;p&gt;Between a skinny and straight fit, our 511&amp;trade; slim fit jeans are cut close without being too restricting. Slim throughout the thigh and leg opening for a long and lean look.&lt;/p&gt;&lt;ul&gt;&lt;li&gt;Slouch1y at top; sits below the waist&lt;/li&gt;&lt;li&gt;Slim through the leg, close at the thigh and straight to the ankle&lt;/li&gt;&lt;li&gt;Stretch for added comfort&lt;/li&gt;&lt;li&gt;Classic five-pocket styling&lt;/li&gt;&lt;li&gt;99% Cotton, 1% Spandex, 11.2 oz. - Imported&lt;/li&gt;&lt;/ul&gt;",
            "show_on_home_page": false,
            "allow_customer_reviews": true,
            "approved_rating_sum": 5,
            "not_approved_rating_sum": 0,
            "approved_total_reviews": 1,
            "not_approved_total_reviews": 0,
            "sku": "LV_511_JN",
            "is_gift_card": false,
            "require_other_products": false,
            "automatically_add_required_products": false,
            "is_download": false,
            "unlimited_downloads": false,
            "max_number_of_downloads": 0,
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
            "allow_adding_only_existing_attribute_combinations": false,
            "disable_buy_button": false,
            "disable_wishlist_button": false,
            "available_for_pre_order": false,
            "call_for_price": false,
            "price": 43.5,
            "old_price": 55,
            "product_cost": 0,
            "customer_enters_price": false,
            "minimum_customer_entered_price": 0,
            "maximum_customer_entered_price": 0,
            "baseprice_enabled": false,
            "baseprice_amount": 0,
            "baseprice_base_amount": 0,
            "has_tier_prices": true,
            "has_discounts_applied": false,
            "weight": 2,
            "length": 2,
            "width": 2,
            "height": 2,
            "display_order": 0,
            "published": true,
            "deleted": false,
            "created_on_utc": "2016-09-30T08:56:23.22",
            "updated_on_utc": "2016-09-30T08:56:23.22",
            "product_type": "SimpleProduct",
            "role_ids": [],
            "discount_ids": [],
            "store_ids": [],
            "manufacturer_ids": [],
            "images": [],
            "tags": [
              "cool",
              "apparel",
              "jeans"
            ],
            "vendor_id": 0
          },
          "product_id": 30
        }
      ],
      "order_status": "Complete",
      "payment_status": "Paid",
      "shipping_status": "Delivered",
      "customer_tax_display_type": "IncludingTax"
    }
  ]
}
```
</p></details>
