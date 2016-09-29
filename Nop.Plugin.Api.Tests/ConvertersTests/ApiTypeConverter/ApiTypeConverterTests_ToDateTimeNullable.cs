using System;
using System.Globalization;
using Nop.Plugin.Api.Converters;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.ConvertersTests.ApiTypeConverter
{
    [TestFixture]
    public class ApiTypeConverterTests_ToDateTimeNullable
    {
        private IApiTypeConverter _apiTypeConverter;

        [SetUp]
        public void SetUp()
        {
            _apiTypeConverter = new Converters.ApiTypeConverter();
        }

        [Test]
        [TestCase("invalid date")]
        [TestCase("20.30.10")]
        [TestCase("2016-30-30")]
        [TestCase("2016/78/12")]
        [TestCase("2016/12/12")]
        [TestCase("2016,12,34")]
        [TestCase("&^%$&(^%$_+")]
        [TestCase("2016,23,07")]
        [TestCase("2016.23.07")]
        [TestCase("2016.07.23")]
        [TestCase("0")]
        public void WhenInvalidDatePassed_ShouldReturnNull(string invalidDate)
        {
            //Arange

            //Act
            DateTime? result = _apiTypeConverter.ToDateTimeNullable(invalidDate);

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
            DateTime? result = _apiTypeConverter.ToDateTimeNullable(nullOrEmpty);

            //Assert
            Assert.IsNull(result);
        }
        
        [Test]
        [TestCase("2016-12-26")]
        [TestCase("2016-12-26T12:45")]
        [TestCase("2016-12-26T12:45:49")]
        [TestCase("2013-12-11T14:36:00+01:00")]
        public void WhenValidDatePassed_ShouldParseThatDate(string validDate)
        {
            //Arange
            DateTime validParsedDate = DateTime.Parse(validDate);

            //Act
            DateTime? result = _apiTypeConverter.ToDateTimeNullable(validDate);

            //Assert
            Assert.AreEqual(validParsedDate, result);
        }
    }
}