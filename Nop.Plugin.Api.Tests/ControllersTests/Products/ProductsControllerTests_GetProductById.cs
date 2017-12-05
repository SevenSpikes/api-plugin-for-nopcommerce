//using System.Web.Http;
//using System.Web.Http.Results;
//using Nop.Core.Domain.Catalog;
//using Nop.Plugin.Api.Controllers;
//using Nop.Plugin.Api.DTOs.Products;
//using Nop.Plugin.Api.MappingExtensions;
//using Nop.Plugin.Api.Serializers;
//using Nop.Plugin.Api.Services;
//using NUnit.Framework;
//using Rhino.Mocks;

//namespace Nop.Plugin.Api.Tests.ControllersTests.Products
//{
//    [TestFixture]
//    public class ProductsControllerTests_GetProductById
//    {
//        [Test]
//        [TestCase(0)]
//        [TestCase(-20)]
//        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveProductId)
//        {
//            // Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializer);

//            // Act
//            IActionResult result = cut.GetProductById(nonPositiveProductId);

//            // Assert
//            Assert.IsInstanceOf<NotFoundResult>(result);
//        }

//        [Test]
//        [TestCase(0)]
//        [TestCase(-20)]
//        public void WhenIdEqualsToZeroOrLess_ShouldNotCallProductApiService(int negativeProductId)
//        {
//            // Arange
//            IProductApiService productApiServiceMock = MockRepository.GenerateMock<IProductApiService>();

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
//            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

//            var cut = new ProductsController(productApiServiceMock, jsonFieldsSerializer);

//            // Act
//            cut.GetProductById(negativeProductId);

//            // Assert
//            productApiServiceMock.AssertWasNotCalled(x => x.GetProductById(negativeProductId));
//        }

//        [Test]
//        public void WhenIdIsPositiveNumberButNoSuchProductExists_ShouldReturn404NotFound()
//        {
//            int nonExistingProductId = 5;

//            // Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productApiServiceStub.Stub(x => x.GetProductById(nonExistingProductId)).Return(null);

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializer);

//            // Act
//            IActionResult result = cut.GetProductById(nonExistingProductId);

//            // Assert
//            Assert.IsInstanceOf<NotFoundResult>(result);
//        }

//        [Test]
//        public void WhenIdEqualsToExistingProductId_ShouldSerializeThatProduct()
//        {
//            Maps.CreateMap<Product, ProductDto>();

//            int existingProductId = 5;
//            var existingProduct = new Product() { Id = existingProductId };

//            // Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productApiServiceStub.Stub(x => x.GetProductById(existingProductId)).Return(existingProduct);

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializer);

//            // Act
//            cut.GetProductById(existingProductId);

//            // Assert
//            jsonFieldsSerializer.AssertWasCalled(
//                x => x.Serialize(
//                    Arg<ProductsRootObjectDto>.Matches(
//                        objectToSerialize =>
//                               objectToSerialize.Products.Count == 1 &&
//                               objectToSerialize.Products[0].Id == existingProduct.Id &&
//                               objectToSerialize.Products[0].Name == existingProduct.Name),
//                    Arg<string>.Is.Equal("")));
//        }

//        [Test]
//        public void WhenIdEqualsToExistingProductIdAndFieldsSet_ShouldReturnJsonForThatProductWithSpecifiedFields()
//        {
//            Maps.CreateMap<Product, ProductDto>();

//            int existingProductId = 5;
//            var existingProduct = new Product() { Id = existingProductId, Name = "some product name" };
//            string fields = "id,name";

//            // Arange
//            IProductApiService productApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productApiServiceStub.Stub(x => x.GetProductById(existingProductId)).Return(existingProduct);

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productApiServiceStub, jsonFieldsSerializer);

//            // Act
//            cut.GetProductById(existingProductId, fields);

//            // Assert
//            jsonFieldsSerializer.AssertWasCalled(
//                x => x.Serialize(
//                    Arg<ProductsRootObjectDto>.Matches(objectToSerialize => objectToSerialize.Products[0].Id == existingProduct.Id &&
//                                                                            objectToSerialize.Products[0].Name == existingProduct.Name),
//                    Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
//        }
//    }
//}