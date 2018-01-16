namespace Nop.Plugin.Api.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using CERTENROLLLib;
    using IdentityModel;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using Nop.Core;

    public static class CryptoHelper
    {
        // Need to ensure that the key would be the same through the application lifetime.
        private static RsaSecurityKey _key;
        private const string TokenSigningCertificateName = "api-token-signing-certificate.pfx";
        private const string TokenSigningKeyFileName = "api-token-signing-key.json";

        // The recomended way to sign a JWT is using a verified certificate!
        public static void CreateSelfSignedCertificate(string subjectName)
        {
            string pathToCertificate = CommonHelper.MapPath($"~/App_Data/{TokenSigningCertificateName}");

            if (!File.Exists(pathToCertificate))
            {
                // create DN for subject and issuer
                var dn = new CX500DistinguishedName();
                dn.Encode("CN=" + subjectName, X500NameFlags.XCN_CERT_NAME_STR_NONE);

                // create a new private key for the certificate
                CX509PrivateKey privateKey = new CX509PrivateKey();
                privateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
                privateKey.MachineContext = true;
                privateKey.Length = 2048;
                privateKey.KeySpec = X509KeySpec.XCN_AT_SIGNATURE; // use is not limited
                privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
                privateKey.Create();

                // Use the stronger SHA512 hashing algorithm
                var hashobj = new CObjectId();
                hashobj.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
                    ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
                    AlgorithmFlags.AlgorithmFlagsNone, "SHA512");

                // add extended key usage if you want - look at MSDN for a list of possible OIDs
                var oid = new CObjectId();
                oid.InitializeFromValue("1.3.6.1.5.5.7.3.1"); // SSL server
                var oidlist = new CObjectIds();
                oidlist.Add(oid);
                var eku = new CX509ExtensionEnhancedKeyUsage();
                eku.InitializeEncode(oidlist);

                // Create the self signing request
                var cert = new CX509CertificateRequestCertificate();
                cert.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");
                cert.Subject = dn;
                cert.Issuer = dn; // the issuer and the subject are the same
                cert.NotBefore = DateTime.Now;
                // this cert expires immediately. Change to whatever makes sense for you
                cert.NotAfter = DateTime.Now;
                cert.X509Extensions.Add((CX509Extension) eku); // add the EKU
                cert.HashAlgorithm = hashobj; // Specify the hashing algorithm
                cert.Encode(); // encode the certificate

                // Do the final enrollment process
                var enroll = new CX509Enrollment();
                enroll.InitializeFromRequest(cert); // load the certificate
                enroll.CertificateFriendlyName = subjectName; // Optional: add a friendly name
                string csr = enroll.CreateRequest(); // Output the request in base64
                // and install it back as the response
                enroll.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate,
                    csr, EncodingType.XCN_CRYPT_STRING_BASE64, ""); // no password
                // output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
                var base64encoded = enroll.CreatePFX("", // no password, this is for internal consumption
                    PFXExportOptions.PFXExportChainWithRoot);

                File.WriteAllBytes(pathToCertificate, Convert.FromBase64String(base64encoded));
            }
        }

        public static X509Certificate2 GetTokenSigningCertificate()
        {
            string pathToCertificate =
                CommonHelper.MapPath($"~/App_Data/{TokenSigningCertificateName}");

            if (File.Exists(pathToCertificate))
            {
                X509Certificate2 certificate = new X509Certificate2(pathToCertificate, "");

                return certificate;
            }

            return null;
        }

        public static RsaSecurityKey CreateRsaSecurityKey()
        {
            if (_key == null)
            {
                string pathToKey = CommonHelper.MapPath($"~/App_Data/{TokenSigningKeyFileName}");

                if (!File.Exists(pathToKey))
                {
                    // generate random parameters
                    var randomParameters = GetRandomParameters();

                    var rsaParams = new RSAParametersWithPrivate();
                    rsaParams.SetParameters(randomParameters);
                    string serializedParameters = JsonConvert.SerializeObject(rsaParams);

                    // create file and save the key
                    File.WriteAllText(pathToKey, serializedParameters);
                }

                // load the key
                if (!File.Exists(pathToKey))
                    throw new FileNotFoundException("Check configuration - cannot find auth key file: " + pathToKey);

                var keyParams = JsonConvert.DeserializeObject<RSAParametersWithPrivate>(File.ReadAllText(pathToKey));

                // create signing key by the key above
                _key = new RsaSecurityKey(keyParams.ToRSAParameters());
            }

            return _key;
        }

        public static RSAParameters GetRandomParameters()
        {       
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    return rsa.ExportParameters(true);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        // https://github.com/mrsheepuk/ASPNETSelfCreatedTokenAuthExample/blob/master/src/TokenAuthExampleWebApplication/RSAKeyUtils.cs
        private class RSAParametersWithPrivate
        {
            public byte[] D { get; set; }
            public byte[] DP { get; set; }
            public byte[] DQ { get; set; }
            public byte[] Exponent { get; set; }
            public byte[] InverseQ { get; set; }
            public byte[] Modulus { get; set; }
            public byte[] P { get; set; }
            public byte[] Q { get; set; }

            public void SetParameters(RSAParameters p)
            {
                D = p.D;
                DP = p.DP;
                DQ = p.DQ;
                Exponent = p.Exponent;
                InverseQ = p.InverseQ;
                Modulus = p.Modulus;
                P = p.P;
                Q = p.Q;
            }
            public RSAParameters ToRSAParameters()
            {
                return new RSAParameters()
                {
                    D = this.D,
                    DP = this.DP,
                    DQ = this.DQ,
                    Exponent = this.Exponent,
                    InverseQ = this.InverseQ,
                    Modulus = this.Modulus,
                    P = this.P,
                    Q = this.Q

                };
            }
        }
    }
}