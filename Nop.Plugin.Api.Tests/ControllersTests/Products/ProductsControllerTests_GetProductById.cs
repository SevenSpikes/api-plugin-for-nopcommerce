//using Nop.Core.Domain.Catalog;
//using Nop.Plugin.Api.Controllers;
//using Nop.Plugin.Api.DTOs.Products;
//using Nop.Plugin.Api.Services;
//using NUnit.Framework;
//using Rhino.Mocks;

//namespace Nop.Plugin.Api.Tests.ControllersTests.Products
//{
//    using AutoMock;
//    using Microsoft.AspNetCore.Mvc;
//    using Nop.Plugin.Api.JSON.Serializers;

//    [TestFixture]
//    public class ProductsControllerTests_GetProductById
//    {
//        [Test]
//        [TestCase(0)]
//        [TestCase(-20)]
//        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveProductId)
//        {
//            // Arange
//            var autoMocker = new RhinoAutoMocker<ProductsController>();

//            // Act
//            IActionResult result = autoMocker.ClassUnderTest.GetProductById(nonPositiveProductId);

//            // Assert
//            Assert.IsInstanceOf<NotFoundResult>(result);
//        }

//        [Test]
//        [TestCase(0)]
//        [TestCase(-20)]
//        public void WhenIdEqualsToZeroOrLess_ShouldNotCallProductApiService(int negativeProductId)
//        {
//            // Arange
//            var autoMocker = new RhinoAutoMocker<ProductsController>();
//            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);

//            // Act
//            autoMocker.ClassUnderTest.GetProductById(negativeProductId);

//            // Assert
//            autoMocker.Get<IProductApiService>().AssertWasNotCalled(x => x.GetProductById());
//        }

//        [Test]
//        public void WhenIdIsPositiveNumberButNoSuchProductExists_ShouldReturn404NotFound()
//        {
//            int nonExistingProductId = 5;

//            // Arange
//            var autoMocker = new RhinoAutoMocker<ProductsController>();
//            autoMocker.Get<IProductApiService>().Stub(x => x.GetProductById(nonExistingProductId)).Return(null);

//            // Act
//            IActionResult result = autoMocker.ClassUnderTest.GetProductById(nonExistingProductId);

//            // Assert
//            Assert.IsInstanceOf<NotFoundObjectResult>(result);
//        }

//        [Test]
//        public void WhenIdEqualsToExistingProductId_ShouldSerializeThatProduct()
//        {
//            int existingProductId = 5;
//            var existingProduct = new Product() { Id = existingProductId };

//            // Arange
//            var autoMocker = new RhinoAutoMocker<ProductsController>();
//            autoMocker.Get<IProductApiService>().Stub(x => x.GetProductById(existingProductId)).Return(existingProduct);

//            // Act
//            autoMocker.ClassUnderTest.GetProductById(existingProductId);

//            // Assert
//            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
//                x => x.Serialize(
//                    Arg<ProductsRootObjectDto>.Matches(
//                        objectToSerialize =>
//                               objectToSerialize.Products.Count == 1 &&
//                               objectToSerialize.Products[0].Id == existingProduct.Id.ToString() &&
//                               objectToSerialize.Products[0].Name == existingProduct.Name),
//                    Arg<string>.Is.Equal("")));
//        }

//        [Test]
//        public void WhenIdEqualsToExistingProductIdAndFieldsSet_ShouldReturnJsonForThatProductWithSpecifiedFields()
//        {
//            int existingProductId = 5;
//            var existingProduct = new Product() { Id = existingProductId, Name = "some product name" };
//            string fields = "id,name";

//            // Arange
//            var autoMocker = new RhinoAutoMocker<ProductsController>();
//            autoMocker.Get<IProductApiService>().Stub(x => x.GetProductById(existingProductId)).Return(existingProduct);

//            // Act
//            autoMocker.ClassUnderTest.GetProductById(existingProductId, fields);

//            // Assert
//            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
//                x => x.Serialize(
//                    Arg<ProductsRootObjectDto>.Matches(objectToSerialize => objectToSerialize.Products[0].Id == existingProduct.Id.ToString() &&
//                                                                            objectToSerialize.Products[0].Name == existingProduct.Name),
//                    Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
//        }
//    }
//}