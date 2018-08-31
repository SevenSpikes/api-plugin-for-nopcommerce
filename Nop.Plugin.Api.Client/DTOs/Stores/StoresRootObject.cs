using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Stores
{
    public class StoresRootObject
    {
        public StoresRootObject()
        {
            Stores = new List<StoreDto>();
        }

        [JsonProperty("stores")]
        public IList<StoreDto> Stores { get; set; }
        
    }
}
