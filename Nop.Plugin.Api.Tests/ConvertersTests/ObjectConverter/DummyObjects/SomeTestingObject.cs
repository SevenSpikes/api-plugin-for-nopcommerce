using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Api.Tests.ConvertersTests.ObjectConverter.DummyObjects
{
    public class SomeTestingObject
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public DateTime? DateTimeNullableProperty { get; set; }
        public bool? BooleanNullableStatusProperty { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public ShippingStatus? ShippingStatus { get; set; }
    }
}