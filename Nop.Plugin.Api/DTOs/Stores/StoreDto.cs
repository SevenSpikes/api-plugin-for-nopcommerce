using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Stores
{
    [JsonObject(Title = "store")]
    public class StoreDto
    {
        /// <summary>
        /// Gets or sets the store ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the store URL
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled
        /// </summary>
        [JsonProperty("ssl_enabled")]
        public bool? SslEnabled { get; set; }

        /// <summary>
        /// Gets or sets the store secure URL (HTTPS)
        /// </summary>
        [JsonProperty("secure_url")]
        public string SecureUrl { get; set; }

        /// <summary>
        /// Gets or sets the comma separated list of possible HTTP_HOST values
        /// </summary>
        [JsonProperty("hosts")]
        public string Hosts { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the default language for this store; 0 is set when we use the default language display order
        /// </summary>
        [JsonProperty("default_language_id")]
        public int? DefaultLanguageId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the company name
        /// </summary>
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company address
        /// </summary>
        [JsonProperty("company_address")]
        public string CompanyAddress { get; set; }

        /// <summary>
        /// Gets or sets the store phone number
        /// </summary>
        [JsonProperty("company_phone_number")]
        public string CompanyPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the company VAT (used in Europe Union countries)
        /// </summary>
        [JsonProperty("company_vat")]
        public string CompanyVat { get; set; }

        /// <summary>
        /// Get or set the currency format
        /// </summary>
        [JsonProperty("primary_currency_display_locale")]
        public string PrimaryCurrencyDisplayLocale { get; set; }
    }
}
