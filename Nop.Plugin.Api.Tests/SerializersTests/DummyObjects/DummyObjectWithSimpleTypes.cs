using Newtonsoft.Json;

namespace Nop.Plugin.Api.Tests.SerializersTests.DummyObjects
{
    public class DummyObjectWithSimpleTypes
    {
        [JsonProperty("first_property")]
        public string FirstProperty { get; set; }

        [JsonProperty("second_property")]
        public string SecondProperty { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DummyObjectWithSimpleTypes)
            {
                var that = obj as DummyObjectWithSimpleTypes;

                return that.FirstProperty.Equals(FirstProperty) && that.SecondProperty.Equals(SecondProperty);
            }

            return false;
        }
    }
}