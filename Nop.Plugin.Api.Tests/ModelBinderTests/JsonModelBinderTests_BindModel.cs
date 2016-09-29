//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Web.Http.Controllers;
//using System.Web.Http.Metadata;
//using System.Web.Http.ModelBinding;
//using Nop.Plugin.Api.Helpers;
//using Nop.Plugin.Api.ModelBinders;
//using NUnit.Framework;
//using Rhino.Mocks;

//namespace Nop.Plugin.Api.Tests.ModelBinderTests
//{
//    [TestFixture]
//    public class JsonModelBinderTests_BindModel
//    {
//        private JsonModelBinder _binder;

//        [SetUp]
//        public void SetUp()
//        {
//            _binder = new JsonModelBinder(new JsonHelper());
//        }

//        [Test]
//        public void WhenRequestDoesNotContainPayload_BindingContextShouldContainInstanceOfTheModelType()
//        {
//            // Arrange
//            var httpControllerContext = new HttpControllerContext();
//            httpControllerContext.Request = new HttpRequestMessage(HttpMethod.Post, "http://someUri");
//            httpControllerContext.Request.Content = new ObjectContent(typeof(Dictionary<string, object>), new Dictionary<string, object>(), new JsonMediaTypeFormatter());

//            var httpActionContext = new HttpActionContext();
//            httpActionContext.ControllerContext = httpControllerContext;

//            var bindingContext = new ModelBindingContext();
//            var provider = MockRepository.GenerateStub<ModelMetadataProvider>();
//            var metaData = new ModelMetadata(provider, null, null, typeof(Dictionary<string, object>), null);
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            _binder.BindModel(httpActionContext, bindingContext);

//            // Assert
//            Assert.IsInstanceOf<Dictionary<string, object>>(bindingContext.Model);
//        }

//        [Test]
//        public void WhenRequestContainsContentWithValidProperty_BindingContextShouldContainInstanceOfTheModelTypeWithItsPropertySetToTheValue()
//        {
//            string categoryName = "test category name";

//            // Arrange
//            var httpControllerContext = new HttpControllerContext();
//            httpControllerContext.Request = new HttpRequestMessage(HttpMethod.Post, "http://someUri");
//            httpControllerContext.Request.Content = new ObjectContent(typeof(Dictionary<string, object>),
//                new Dictionary<string, object>()
//                {
//                    {
//                        "name",
//                        categoryName
//                    }
//                },
//                new JsonMediaTypeFormatter());

//            var httpActionContext = new HttpActionContext();
//            httpActionContext.ControllerContext = httpControllerContext;

//            var bindingContext = new ModelBindingContext();
//            var provider = MockRepository.GenerateStub<ModelMetadataProvider>();
//            var metaData = new ModelMetadata(provider, null, null, typeof(Dictionary<string, object>), null);
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            _binder.BindModel(httpActionContext, bindingContext);

//            // Assert
//            Assert.AreEqual(categoryName, ((Dictionary<string, object>)bindingContext.Model)["name"]);
//        }
//    }
//}