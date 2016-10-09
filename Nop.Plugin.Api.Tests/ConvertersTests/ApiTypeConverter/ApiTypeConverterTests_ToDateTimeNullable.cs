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
            DateTime? result = _apiTypeConverter.ToUtcDateTimeNullable(invalidDate);

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
            DateTime? result = _apiTypeConverter.ToUtcDateTimeNullable(nullOrEmpty);

            //Assert
            Assert.IsNull(result);
        }
        
        [Test]
        [TestCase("2016-12")]
        [TestCase("2016-12-26")]
        [TestCase("2016-12-26T06:45")]
        [TestCase("2016-12-26T06:45:49")]
        [TestCase("2016-12-26T06:45:49.05")]
        public void WhenValidIso8601DateWithoutTimeZoneOrOffsetPassed_ShouldConvertAsUTC(string validDate)
        {
            //Arange
            DateTime expectedDateTimeUtc = DateTime.Parse(validDate,null,DateTimeStyles.RoundtripKind);

            //Act
            DateTime? result = _apiTypeConverter.ToUtcDateTimeNullable(validDate);

            //Assert
            Assert.AreEqual(expectedDateTimeUtc, result);
        }

        [TestCase("2016-12-26T06:45:49Z")]
        [TestCase("2016-12-26T07:45:49+01:00")]
        [TestCase("2016-12-26T08:45:49+02:00")]
        [TestCase("2016-12-26T04:45:49-02:00")]
        public void WhenValidDateWithTimeZoneOrOffsetPassed_ShouldConvertThatDateInUTC(string validDate)
        {
            //Arange
            DateTime expectedDateTimeUtc = new DateTime(2016,12,26,6,45,49,DateTimeKind.Utc);

            //Act
            DateTime? result = _apiTypeConverter.ToUtcDateTimeNullable(validDate);

            //Assert
            Assert.AreEqual(expectedDateTimeUtc, result);
        }
    }
}