//using System.Collections.Generic;
//using System.Web.Http;
//using System.Web.Http.Results;
//using Nop.Core.Domain.Catalog;
//using Nop.Plugin.Api.Constants;
//using Nop.Plugin.Api.Controllers;
//using Nop.Plugin.Api.DTOs.Products;
//using Nop.Plugin.Api.MappingExtensions;
//using Nop.Plugin.Api.Models.ProductsParameters;
//using Nop.Plugin.Api.Serializers;
//using Nop.Plugin.Api.Services;
//using NUnit.Framework;
//using Rhino.Mocks;

//namespace Nop.Plugin.Api.Tests.ControllersTests.Products
//{
//    [TestFixture]
//    public class ProductsControllerTests_GetProducts
//    {
//        [Test]
//        [TestCase(Configurations.MinLimit - 1)]
//        [TestCase(Configurations.MaxLimit + 1)]
//        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
//        {
//            var parameters = new ProductsParametersModel()
//            {
//                Limit = invalidLimit
//            };

//            //Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();

//            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializerStub);

//            //Act
//            IHttpActionResult result = cut.GetProducts(parameters);

//            //Assert
//            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
//        }

//        [Test]
//        [TestCase(-1)]
//        [TestCase(0)]
//        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
//        {
//            var parameters = new ProductsParametersModel()
//            {
//                Page = invalidPage
//            };

//            //Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();

//            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializerStub);

//            //Act
//            IHttpActionResult result = cut.GetProducts(parameters);

//            //Assert
//            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
//        }

//        [Test]
//        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
//        {
//            var parameters = new ProductsParametersModel();

//            //Arange
//            IProductApiService productsApiServiceMock = MockRepository.GenerateMock<IProductApiService>();

//            productsApiServiceMock.Expect(x => x.GetProducts(parameters.Ids,
//                                                    parameters.CreatedAtMin,
//                                                    parameters.CreatedAtMax,
//                                                    parameters.UpdatedAtMin,
//                                                    parameters.UpdatedAtMax,
//                                                    parameters.Limit,
//                                                    parameters.Page,
//                                                    parameters.SinceId,
//                                                    parameters.CategoryId,
//                                                    parameters.VendorName,
//                                                    parameters.PublishedStatus)).Return(new List<Product>());

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productsApiServiceMock, jsonFieldsSerializer);

//            //Act
//            cut.GetProducts(parameters);

//            //Assert
//            productsApiServiceMock.VerifyAllExpectations();
//        }

//        [Test]
//        public void WhenNoProductsExist_ShouldCallTheSerializerWithNoProducts()
//        {
//            var returnedProductsCollection = new List<Product>();

//            var parameters = new ProductsParametersModel();

//            //Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productApiServiceStub.Stub(x => x.GetProducts()).Return(returnedProductsCollection);

//            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializerMock);

//            //Act
//            cut.GetProducts(parameters);

//            //Assert
//            jsonFieldsSerializerMock.AssertWasCalled(
//                x => x.Serialize(Arg<ProductsRootObjectDto>.Matches(r => r.Products.Count == returnedProductsCollection.Count),
//                Arg<string>.Is.Equal(parameters.Fields)));
//        }

//        [Test]
//        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
//        {
//            var parameters = new ProductsParametersModel()
//            {
//                Fields = "id,name"
//            };

//            //Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productApiServiceStub.Stub(x => x.GetProducts()).Return(new List<Product>());

//            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializerMock);

//            //Act
//            cut.GetProducts(parameters);

//            //Assert
//            jsonFieldsSerializerMock.AssertWasCalled(
//                x => x.Serialize(Arg<ProductsRootObjectDto>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
//        }

//        [Test]
//        public void WhenSomeProductsExist_ShouldCallTheSerializerWithTheseProducts()
//        {
//            Maps.CreateMap<Product, ProductDto>();

//            var returnedProductsDtoCollection = new List<Product>()
//            {
//                new Product(),
//                new Product()
//            };

//            var parameters = new ProductsParametersModel();

//            //Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productApiServiceStub.Stub(x => x.GetProducts()).Return(returnedProductsDtoCollection);

//            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializerMock);

//            //Act
//            cut.GetProducts(parameters);

//            //Assert
//            jsonFieldsSerializerMock.AssertWasCalled(
//                x => x.Serialize(Arg<ProductsRootObjectDto>.Matches(r => r.Products.Count == returnedProductsDtoCollection.Count),
//                Arg<string>.Is.Equal(parameters.Fields)));
//        }
//    }
//}