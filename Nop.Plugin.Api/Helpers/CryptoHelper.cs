namespace Nop.Plugin.Api.Helpers
{
    using System.Security.Cryptography;
    using IdentityModel;
    using Microsoft.IdentityModel.Tokens;

    public static class CryptoHelper
    {
        // Need to ensure that the key would be the same through the application lifetime.
        private static RsaSecurityKey _key = null;

        public static RsaSecurityKey CreateRsaSecurityKey()
        {
            if (_key == null)
            {
                var rsa = RSA.Create();

                if (rsa is RSACryptoServiceProvider)
                {
                    rsa.Dispose();
                    var cng = new RSACng(2048);

                    var parameters = cng.ExportParameters(includePrivateParameters: true);
                    _key = new RsaSecurityKey(parameters);
                }
                else
                {
                    rsa.KeySize = 2048;
                    _key = new RsaSecurityKey(rsa);
                }

                _key.KeyId = CryptoRandom.CreateUniqueId(16);
            }

            return _key;
        }
    }
}