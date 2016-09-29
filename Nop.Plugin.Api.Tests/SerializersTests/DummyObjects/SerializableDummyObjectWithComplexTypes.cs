using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs;

namespace Nop.Plugin.Api.Tests.SerializersTests.DummyObjects
{
    public class SerializableDummyObjectWithComplexTypes : ISerializableObject
    {
        public SerializableDummyObjectWithComplexTypes()
        {
            Items = new List<DummyObjectWithComplexTypes>();
        }

        [JsonProperty("primary_complex_property")]
        public IList<DummyObjectWithComplexTypes> Items { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "primary_complex_property";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(DummyObjectWithComplexTypes);
        }
    }
}