using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    internal static class InternalConfigurationClient
    {
        // ReSharper disable ClassNeverInstantiated.Local - For deserialization
        // ReSharper disable InconsistentNaming
#pragma warning disable 649
        private class OpenIdConfiguration
        {
            public string issuer;
            public string access_token_issuer;
            public string jwks_uri;
        }

        private class JwksConfiguration
        {
            public JwksConfigurationKey[] keys;
        }

        private class JwksConfigurationKey
        {
            public string[] x5c;
        }
#pragma warning restore 649
        // ReSharper restore InconsistentNaming
        // ReSharper restore ClassNeverInstantiated.Local

        public static async Task<OpenIdConnectConfiguration> RequestConfigurationAsync(
            string metadataAddress, Func<IHttpClient> createClient)
        {
            using (IHttpClient client = createClient())
            {
                OpenIdConfiguration configuration = await GetOpenidConfiguration(metadataAddress, client);
                string issuer = GetIssuer(configuration);
                var certificate = await GetCertificate(configuration, client);
                return new OpenIdConnectConfiguration
                {
                    Issuer = issuer,
                    Certificate = certificate
                };
            }
        }

        private static string GetIssuer(OpenIdConfiguration configuration)
        {
            return configuration.access_token_issuer ?? configuration.issuer;
        }

        private static async Task<OpenIdConfiguration> GetOpenidConfiguration(string metadataAddress, IHttpClient client)
        {
            string json = await client.GetStringAsync(metadataAddress);
            return JsonConvert.DeserializeObject<OpenIdConfiguration>(json);
        }

        private static async Task<string> GetCertificate(OpenIdConfiguration configuration, IHttpClient client)
        {
            string jwksUri = configuration.jwks_uri;
            if (string.IsNullOrWhiteSpace(jwksUri))
                throw CreateException("jwks_uri");
            JwksConfiguration jwks = await GetJwksConfiguration(client, jwksUri);
            var keys = jwks.keys;
            if (keys == null || keys.Length < 1)
                throw CreateException("keys");
            var key = keys[0];
            var certificates = key.x5c;
            if (certificates == null || certificates.Length < 1)
                throw CreateException("x5c");
            return certificates[0];
        }

        private static OpenIdConfigurationMissingFieldException CreateException(string missingField)
        {
            return new OpenIdConfigurationMissingFieldException(missingField);
        }

        private static async Task<JwksConfiguration> GetJwksConfiguration(IHttpClient client, string jwksUri)
        {
            string json = await client.GetStringAsync(jwksUri);
            return JsonConvert.DeserializeObject<JwksConfiguration>(json);
        }
    }
}
