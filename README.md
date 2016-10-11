# API plugin for nopCommerce

This plugin provides a RESTful API for managing resources in nopCommerce.

What is a RESTful API?
----------------------------------------

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

What can you do with Customers?
---------------------------------

The nopCommerce API lets you do the following with the Customer resource.

+ GET /api/customers  
Receive a list of all Customers

+ GET /api/customers/search?query=first_name:john  
Search for customers matching supplied query

+ GET /api/customers/{id}  
Receive a single Customer

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

Customer Endpoints
-------------------------------------

|  GET |  /api/customers |
|:---|:---|
|  since_id |  Restrict results to after the specified ID |
|  created_at_min |  Show customers created after date (format: 2014-04-25T16:15:47-04:00) |
|  created_at_max |  Show customers created before date (format: 2014-04-25T16:15:47-04:00) |
|  limit |  Amount of results (default: 50) (maximum: 250) |
|  page |  Page to show (default: 1) |
|  fields |  Comma-separated list of fields to include in the response |

