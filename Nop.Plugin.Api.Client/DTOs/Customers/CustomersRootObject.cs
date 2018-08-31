using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Customers
{
    public class CustomersRootObject 
    {
        public CustomersRootObject()
        {
            Customers = new List<CustomerDto>();    
        }
        
        [JsonProperty("customers")]
        public IList<CustomerDto> Customers { get; set; }

    }
}