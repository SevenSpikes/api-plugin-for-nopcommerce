//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using Nop.Plugin.Api.Converters;
//using Nop.Plugin.Api.ModelBinders;
//using Nop.Plugin.Api.Tests.ModelBinderTests.DummyObjects;
//using NUnit.Framework;
//using Rhino.Mocks;

//namespace Nop.Plugin.Api.Tests.ModelBinderTests
//{
//    using Microsoft.AspNetCore.Mvc.Internal;
//    using Microsoft.AspNetCore.Mvc.ModelBinding;
//    using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

//    [TestFixture]
//    public class ParametersModelBinderTests_BindModel
//    {
//        private ParametersModelBinder<DummyModel> _binder;

//        [SetUp]
//        public void SetUp()
//        {
//            IApiTypeConverter apiTypeConverter = new ApiTypeConverter();
//            IObjectConverter objectConverter = new ObjectConverter(apiTypeConverter);
//            _binder = new ParametersModelBinder<DummyModel>(objectConverter);
//        }

//        [Test]
//        public void WhenRequestDoesNotContainQuery_BindingContextShouldContainInstanceOfTheModelType()
//        {
//            // Arrange
//            var bindingContext = new DefaultModelBindingContext();
//            var modelProvider = MockRepository.GenerateStub<DefaultModelMetadataProvider>();
//            var detailsProvider = MockRepository.GenerateStub<DefaultCompositeMetadataDetailsProvider>();
//            var metaData = new DefaultModelMetadata(modelProvider, detailsProvider, new DefaultMetadataDetails());
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            _binder.BindModelAsync(bindingContext);

//            // Assert
//            Assert.IsInstanceOf<DummyModel>(bindingContext.Model);
//        }

//        [Test]
//        public void WhenRequestContainsQueryWithValidIntProperty_BindingContextShouldContainInstanceOfTheModelTypeWithItsIntPropertySetToTheValue()
//        {
//            // Arrange
//            var httpControllerContext = new HttpControllerContext();
//            httpControllerContext.Request = new HttpRequestMessage(HttpMethod.Get, "http://someUri");
//            httpControllerContext.Request.Content = new ObjectContent(typeof(DummyModel), new DummyModel(), new XmlMediaTypeFormatter());

//            httpControllerContext.Request.Properties.Add("MS_QueryNameValuePairs", new List<KeyValuePair<string, string>>()
//            {
//                new KeyValuePair<string, string>("int_property", "5")
//            });

//            var httpActionContext = new HttpActionContext();
//            httpActionContext.ControllerContext = httpControllerContext;

//            var bindingContext = new ModelBindingContext();
//            var provider = MockRepository.GenerateStub<ModelMetadataProvider>();
//            var metaData = new ModelMetadata(provider, null, null, typeof(DummyModel), null);
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            _binder.BindModel(httpActionContext, bindingContext);

//            // Assert
//            Assert.AreEqual(5, ((DummyModel)bindingContext.Model).IntProperty);
//        }

//        [Test]
//        public void WhenRequestContainsQueryWithValidStringProperty_BindingContextShouldContainInstanceOfTheModelTypeWithItsStringPropertySetToTheValue()
//        {
//            // Arrange
//            var httpControllerContext = new HttpControllerContext();
//            httpControllerContext.Request = new HttpRequestMessage(HttpMethod.Get, "http://someUri");
//            httpControllerContext.Request.Content = new ObjectContent(typeof(DummyModel), new DummyModel(), new XmlMediaTypeFormatter());

//            httpControllerContext.Request.Properties.Add("MS_QueryNameValuePairs", new List<KeyValuePair<string, string>>()
//            {
//                new KeyValuePair<string, string>("string_property", "some value")
//            });

//            var httpActionContext = new HttpActionContext();
//            httpActionContext.ControllerContext = httpControllerContext;

//            var bindingContext = new ModelBindingContext();
//            var provider = MockRepository.GenerateStub<ModelMetadataProvider>();
//            var metaData = new ModelMetadata(provider, null, null, typeof(DummyModel), null);
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            _binder.BindModel(httpActionContext, bindingContext);

//            // Assert
//            Assert.AreEqual("some value", ((DummyModel)bindingContext.Model).StringProperty);
//        }

//        [Test]
//        public void WhenRequestContainsQueryWithValidDateTimeProperty_BindingContextShouldContainInstanceOfTheModelTypeWithItsDateTimePropertySetToTheValue()
//        {
//            // Arrange
//            var httpControllerContext = new HttpControllerContext();
//            httpControllerContext.Request = new HttpRequestMessage(HttpMethod.Get, "http://someUri");
//            httpControllerContext.Request.Content = new ObjectContent(typeof(DummyModel), new DummyModel(), new XmlMediaTypeFormatter());

//            httpControllerContext.Request.Properties.Add("MS_QueryNameValuePairs", new List<KeyValuePair<string, string>>()
//            {
//                new KeyValuePair<string, string>("date_time_nullable_property", "2016-12-12")
//            });

//            var httpActionContext = new HttpActionContext();
//            httpActionContext.ControllerContext = httpControllerContext;

//            var bindingContext = new ModelBindingContext();
//            var provider = MockRepository.GenerateStub<ModelMetadataProvider>();
//            var metaData = new ModelMetadata(provider, null, null, typeof(DummyModel), null);
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            _binder.BindModel(httpActionContext, bindingContext);

//            // Assert
//            Assert.AreEqual(new DateTime(2016, 12, 12), ((DummyModel)bindingContext.Model).DateTimeNullableProperty.Value);
//        }

//        [Test]
//        public void WhenRequestContainsQueryWithValidBooleanStatusProperty_BindingContextShouldContainInstanceOfTheModelTypeWithItBooleanStatusPropertySetToTheValue()
//        {
//            // Arrange
//            var httpControllerContext = new HttpControllerContext();
//            httpControllerContext.Request = new HttpRequestMessage(HttpMethod.Get, "http://someUri");
//            httpControllerContext.Request.Content = new ObjectContent(typeof(DummyModel), new DummyModel(), new XmlMediaTypeFormatter());

//            httpControllerContext.Request.Properties.Add("MS_QueryNameValuePairs", new List<KeyValuePair<string, string>>()
//            {
//                new KeyValuePair<string, string>("boolean_nullable_status_property", "published")
//            });

//            var httpActionContext = new HttpActionContext();
//            httpActionContext.ControllerContext = httpControllerContext;

//            var bindingContext = new ModelBindingContext();
//            var provider = MockRepository.GenerateStub<ModelMetadataProvider>();
//            var metaData = new ModelMetadata(provider, null, null, typeof(DummyModel), null);
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            _binder.BindModel(httpActionContext, bindingContext);

//            // Assert
//            Assert.AreEqual(true, ((DummyModel)bindingContext.Model).BooleanNullableStatusProperty.Value);
//        }

//        [Test]
//        public void BindModel_ShouldAlwaysReturnTrue()
//        {
//            // Arrange
//            var httpControllerContext = new HttpControllerContext();
//            httpControllerContext.Request = new HttpRequestMessage(HttpMethod.Get, "http://someUri");
//            httpControllerContext.Request.Content = new ObjectContent(typeof(DummyModel), new DummyModel(), new XmlMediaTypeFormatter());
            
//            var httpActionContext = new HttpActionContext();
//            httpActionContext.ControllerContext = httpControllerContext;

//            var bindingContext = new ModelBindingContext();
//            var provider = MockRepository.GenerateStub<ModelMetadataProvider>();
//            var metaData = new ModelMetadata(provider, null, null, typeof(DummyModel), null);
//            bindingContext.ModelMetadata = metaData;

//            //Act
//            bool result = _binder.BindModel(httpActionContext, bindingContext);

//            // Assert
//            Assert.IsTrue(result);
//        }
//    }
//}