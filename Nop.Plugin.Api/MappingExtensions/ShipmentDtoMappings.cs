using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTOs.Shipments;
using Nop.Plugin.Api.DTOs.Stores;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ShipmentDtoMappings
    {
        public static ShipmentDto ToDto(this Shipment shipment)
        {
            return shipment.MapTo<Shipment, ShipmentDto>();
        }
    }
}
