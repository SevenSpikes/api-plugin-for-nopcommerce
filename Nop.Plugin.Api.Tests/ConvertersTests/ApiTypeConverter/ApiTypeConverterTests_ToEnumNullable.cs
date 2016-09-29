using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Converters;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ConvertersTests.ApiTypeConverter
{
    [TestFixture]
    public class ApiTypeConverterTests_ToEnumNullable
    {
        private IApiTypeConverter _apiTypeConverter;

        [SetUp]
        public void SetUp()
        {
            _apiTypeConverter = new Converters.ApiTypeConverter();
        }

        [Test]
        [TestCase("invalid order status", typeof(OrderStatus?))]
        [TestCase("-100", typeof(OrderStatus?))]
        [TestCase("345345345345345345345345345", typeof(OrderStatus?))]
        [TestCase("invalid payment status", typeof(PaymentStatus?))]
        [TestCase("-10", typeof(PaymentStatus?))]
        [TestCase("34534343543456767", typeof(PaymentStatus?))]
        [TestCase("invalid shipping status", typeof(ShippingStatus?))]
        [TestCase("-17800", typeof(ShippingStatus?))]
        [TestCase("546546545454546", typeof(ShippingStatus?))]
        [TestCase("$%%#@@$%^^))_(!34sd", typeof(OrderStatus?))]
        public void WhenInvalidEnumPassed_GivenTheEnumType_ShouldReturnNull(string invalidOrderStatus, Type type)
        {
            //Arange

            //Act
            var result = _apiTypeConverter.ToEnumNullable(invalidOrderStatus, type);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void WhenNullOrEmptyStringPassed_ShouldReturnNull(string nullOrEmpty)
        {
            //Arange

            //Act
            var result = _apiTypeConverter.ToEnumNullable(Arg<string>.Is.Equal(nullOrEmpty), Arg<Type>.Is.Anything);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase("Pending", typeof(OrderStatus?))]
        [TestCase("Authorized", typeof(PaymentStatus?))]
        [TestCase("NotYetShipped", typeof(ShippingStatus?))]
        [TestCase("pending", typeof(OrderStatus?))]
        [TestCase("authorized", typeof(PaymentStatus?))]
        [TestCase("notyetshipped", typeof(ShippingStatus?))]
        public void WhenValidEnumValuePassed_GivenTheEnumType_ShouldParseThatValueToEnum(string validEnum, Type type)
        {
            //Arange
            var enumValueParsed = Enum.Parse(Nullable.GetUnderlyingType(type), validEnum, true);

            //Act
            var result = _apiTypeConverter.ToEnumNullable(validEnum, type);

            //Assert
            Assert.AreEqual(enumValueParsed, result);
        }
    }
}