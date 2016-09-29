//using System.Web.Http;
//using System.Web.Http.Results;
//using Nop.Plugin.Api.Controllers;
//using Nop.Plugin.Api.DTOs.Products;
//using Nop.Plugin.Api.Models.ProductsParameters;
//using Nop.Plugin.Api.Serializers;
//using Nop.Plugin.Api.Services;
//using NUnit.Framework;
//using Rhino.Mocks;

//namespace Nop.Plugin.Api.Tests.ControllersTests.Products
//{
//    [TestFixture]
//    public class ProductsControllerTests_GetProductsCount
//    {
//        [Test]
//        public void WhenNoProductsExist_ShouldReturnOKResultWithCountEqualToZero()
//        {
//            var parameters = new ProductsCountParametersModel();

//            // arrange
//            var productsApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productsApiServiceStub.Stub(x => x.GetProductsCount()).IgnoreArguments().Return(0);

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productsApiServiceStub, jsonFieldsSerializer);

//            // act
//            IHttpActionResult result = cut.GetProductsCount(parameters);

//            // assert
//            Assert.IsInstanceOf<OkNegotiatedContentResult<ProductsCountRootObject>>(result);
//            Assert.AreEqual(0, ((OkNegotiatedContentResult<ProductsCountRootObject>)result).Content.Count);
//        }

//        [Test]
//        public void WhenSingleProductExists_ShouldReturnOKWithCountEqualToOne()
//        {
//            var parameters = new ProductsCountParametersModel();

//            // arrange
//            var productsApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productsApiServiceStub.Stub(x => x.GetProductsCount()).IgnoreArguments().Return(1);

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productsApiServiceStub, jsonFieldsSerializer);

//            // act
//            IHttpActionResult result = cut.GetProductsCount(parameters);

//            // assert
//            Assert.IsInstanceOf<OkNegotiatedContentResult<ProductsCountRootObject>>(result);
//            Assert.AreEqual(1, ((OkNegotiatedContentResult<ProductsCountRootObject>)result).Content.Count);
//        }

//        [Test]
//        public void WhenCertainNumberOfProductsExist_ShouldReturnOKWithCountEqualToSameNumberOfProducts()
//        {
//            var productsCountParametersModel = new ProductsCountParametersModel();
//            int productsCount = 20;

//            // arrange
//            var productsApiServiceStub = MockRepository.GenerateStub<IProductApiService>();
//            productsApiServiceStub.Stub(x => x.GetProductsCount()).IgnoreArguments().Return(productsCount);

//            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

//            var cut = new ProductsController(productsApiServiceStub, jsonFieldsSerializer);

//            // act
//            IHttpActionResult result = cut.GetProductsCount(productsCountParametersModel);

//            // assert
//            Assert.IsInstanceOf<OkNegotiatedContentResult<ProductsCountRootObject>>(result);
//            Assert.AreEqual(productsCount, ((OkNegotiatedContentResult<ProductsCountRootObject>)result).Content.Count);
//        }
//    }
//}