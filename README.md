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

[**Customers**](Customers.md)

[**Products**](Products.md)

[**Categories**](Categories.md)

[**ProductCategoryMappings**](ProductCategoryMappings.md)

[**Orders**](Orders.md)

[**OrderItems**](OrderItems.md)

[**ShoppingCartItems**](ShoppingCartItems.md)

With the nopCommerce API, you can perform any of the four CRUD actions against any of your nopCommerce site’s resources listed above. For example, you can use the API to create a product, retrieve a product, update a product or delete a product associated with your nopCommerce website.

## What about security?

The API plugin currently supports OAuth 2.0 Authorization Code grant type flow. So in order to access the resource endpoints you need to provide a valid AccessToken. To understand how the authorization code grant flow works please refer to the [**Sample Application**](https://github.com/SevenSpikes/nopCommerce-Api-SampleApplication).
