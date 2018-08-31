using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs
{
    [JsonObject(Title = "address")]
    public class AddressDto
    {
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the company
        /// </summary>
        [JsonProperty("company")]
        public string Company { get; set; }
        
        /// <summary>
        /// Gets or sets the country name
        /// </summary>
        [JsonProperty("country")]
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the address 1
        /// </summary>
        [JsonProperty("address1")]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2
        /// </summary>
        [JsonProperty("address2")]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the zip/postal code
        /// </summary>
        [JsonProperty("zip_postal_code")]
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the fax number
        /// </summary>
        [JsonProperty("fax_number")]
        public string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the state/province
        /// </summary>
        [JsonProperty("province")]
        public string StateProvinceName { get; set; }
    }
}