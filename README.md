# API plugin for nopCommerce

This plugin provides a RESTful API for managing resources in nopCommerce.

## What is a RESTful API?


HTTP requests are often the way that you interact with a RESTful API.
A client makes an HTTP request to a server and the server responds with an HTTP response.

In a HTTP request, you need to define the type of action that you want to perform against a resource. There are four primary actions associated with any HTTP request (commonly referred to as CRUD):

**POST** (Create)

**GET** (Retrieve)

**PUT** (Update)

**DELETE** (Delete)

A resource is a data object that can be accessed via an HTTP request. The API allows you to “access your nopCommerce site’s data (resources) through an easy-to-use HTTP REST API”. In the case of the most recent version of the API (nopCommerce version 3.80), the resources include the following 7 nopCommerce objects:

[**Customers**](#what-can-you-do-with-customers)

**Products**

**Categories**

**ProductCategoryMappings**

**Orders**

**OrderItems**

**ShoppingCartItems**

With the nopCommerce API, you can perform any of the four CRUD actions against any of your nopCommerce site’s resources listed above. For example, you can use the API to create a product, retrieve a product, update a product or delete a product associated with your nopCommerce website.

## What can you do with Customers?

The nopCommerce API lets you do the following with the Customer resource.

+ [GET /api/customers  
Receive a list of all Customers](#get-apicustomers)

+ [GET /api/customers/search?query=first_name:john  
Search for customers matching supplied query](#get-apicustomerssearch)

+ [GET /api/customers/{id}  
Receive a single Customer](#get-apicustomersid)

+ POST /api/customers  
Create a new Customer

+ PUT /api/customers/{id}  
Modify an existing Customer

+ DELETE /api/customers/{id}  
Remove a Customer (mark as Deleted)

+ GET /api/customers/count  
Receive a count of all Customers

+ GET /api/orders/customer/{customer_id}  
Find orders belonging to this customer

## Customer Endpoints


### GET /api/customers  
Retrieve all customers

|  GET |  /api/customers |
|:---|:---|
|  since_id |  Restrict results to after the specified ID |
|  created_at_min |  Show customers created after date (format: 2014-04-25T16:15:47-04:00) |
|  created_at_max |  Show customers created before date (format: 2014-04-25T16:15:47-04:00) |
|  limit |  Amount of results (default: 50) (maximum: 250) |
|  page |  Page to show (default: 1) |
|  fields |  Comma-separated list of fields to include in the response |

#### GET /api/customers  
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

#### GET /api/customers?updated_at_min=2016-09-30T08:56:13.85  
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

---

### GET /api/customers/search  
Search for customers matching supplied query

|  GET |  /api/customers/search |
|:---|:---|
|  order |  Field and direction to order results by (default: id DESC) |
|  query |  Text to search customers |
|  limit |  Amount of results (default: 50) (maximum: 250) |
|  page |  Page to show (default: 1) |
|  fields |  Comma-separated list of fields to include in the response |

#### GET /api/customers/search?query=first_name:john  
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

---

### GET /api/customers/{id}  
Retrieve customer by specified id

|  GET |  /api/customers/{id} |
|:---|:---|
|  fields |  Comma-separated list of fields to include in the response |

#### GET /api/customers/1  
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

#### GET /api/customers/1?fields=id,email  
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

---

### POST /api/customers  

#### Trying to create a customer without an email or customer role will return an error
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

#### Create a new customer record
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

