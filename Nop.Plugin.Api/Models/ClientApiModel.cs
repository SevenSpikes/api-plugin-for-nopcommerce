namespace Nop.Plugin.Api.Models
{
    using System;
    using Nop.Plugin.Api.Constants;

    public class ClientApiModel
    {
        private int _accessTokenLifetime;
        private int _refreshTokenLifetime;
        private string _clientId;
        private string _clientSecretRaw;

        public int Id { get; set; }
        public string ClientName { get; set; }

        public string ClientId
        {
            get
            {
                if (string.IsNullOrEmpty(_clientId))
                {
                    _clientId = Guid.NewGuid().ToString();
                }

                return _clientId;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _clientId = Guid.NewGuid().ToString();
                }
                else
                {
                    _clientId = value;
                }
            }
        }

        public string ClientSecretDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_clientSecretRaw))
                {
                    _clientSecretRaw = Guid.NewGuid().ToString();
                }

                return _clientSecretRaw;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _clientSecretRaw = Guid.NewGuid().ToString();
                }
                else
                {
                    _clientSecretRaw = value;
                }
            }
        }

        public string RedirectUrl { get; set; }

        public int AccessTokenLifetime
        {
            get
            {
                if (_accessTokenLifetime <= 0)
                {
                    _accessTokenLifetime = Configurations.DefaultAccessTokenExpiration;
                }

                return _accessTokenLifetime;
            }
            set
            {
                if (value <= 0)
                {
                    _accessTokenLifetime = Configurations.DefaultAccessTokenExpiration;
                }
                else
                {
                    _accessTokenLifetime = value;
                }
            }
        }

        public int RefreshTokenLifetime
        {
            get
            {
                if (_refreshTokenLifetime <= 0)
                {
                    _refreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration;
                }

                return _refreshTokenLifetime;
            }
            set
            {
                if (value <= 0)
                {
                    _refreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration;
                }
                else
                {
                    _refreshTokenLifetime = value;
                }
            }
        }

        public bool Enabled { get; set; }
    }
}