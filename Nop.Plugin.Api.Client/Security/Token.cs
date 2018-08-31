// -----------------------------------------------------------------------
// <copyright from="2018" to="2018" file="Token.cs" company="Lindell Technologies">
//    Copyright (c) Lindell Technologies All Rights Reserved.
//    Information Contained Herein is Proprietary and Confidential.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.Security
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return AccessToken;
        }
    }

}