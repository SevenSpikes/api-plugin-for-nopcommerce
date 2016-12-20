using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Nop.Plugin.Api.DTOs.Products;
using NUnit.Framework;
using Nop.Plugin.Api.Tests.SerializersTests.DummyObjects;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.Tests.ValidatorTests
{
    public class TypeValidatorTests_IsValid
    {
        [Test]
        [SetCulture("de-de")]
        [Description("Regression test for issue #11 - https://github.com/SevenSpikes/api-plugin-for-nopcommerce/issues/11")]
        public void WhenCurrentCultureUsesCommaAsDecimalPoint_ShouldProperlyValidateProductPrice()
        {
            //Arange
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("price", 33.33);

            var cut = new TypeValidator<ProductDto>();

            //Act
            bool result = cut.IsValid(properties);

            // Assert
            Assert.IsTrue(result);
        }
    }
}