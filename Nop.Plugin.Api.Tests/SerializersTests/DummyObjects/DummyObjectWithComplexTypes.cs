using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Tests.SerializersTests.DummyObjects
{
    public class DummyObjectWithComplexTypes
    {
        [JsonProperty("string_property")]
        public string StringProperty { get; set; }

        [JsonProperty("int_property")]
        public int IntProperty { get; set; }

        [JsonProperty("bool_property")]
        public bool BoolProperty { get; set; }

        [JsonProperty("list_of_dummy_object_with_simple_types")]
        public IList<DummyObjectWithSimpleTypes> ListOfDummyObjectWithSimpleTypes { get; set; }

        [JsonProperty("dummy_object_with_simple_types")]
        public DummyObjectWithSimpleTypes DummyObjectWithSimpleTypes { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DummyObjectWithComplexTypes)
            {
                var that = obj as DummyObjectWithComplexTypes;

                return that.StringProperty.Equals(StringProperty) &&
                       that.IntProperty == IntProperty &&
                       that.BoolProperty == BoolProperty &&
                       that.ListOfDummyObjectWithSimpleTypes.Equals(ListOfDummyObjectWithSimpleTypes) &&
                       that.DummyObjectWithSimpleTypes.Equals(DummyObjectWithSimpleTypes);
            }

            return false;
        }
    }
}