using System;

namespace Nop.Plugin.Api.Tests.ModelBinderTests.DummyObjects
{
    public class DummyModel
    {
         public string StringProperty { get; set; }
         public int IntProperty { get; set; }
         public int? IntNullableProperty { get; set; }
        public DateTime? DateTimeNullableProperty { get; set; }
         public bool? BooleanNullableStatusProperty { get; set; }
    }
}