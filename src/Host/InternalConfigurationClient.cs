using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    internal static class InternalConfigurationClient
    {
        public static async Task<OpenIdConnectConfiguration> RequestConfigurationAsync(string metadataAddress, Func<IHttpClient> createClient)
        {
            using (var client = createClient())
            {
                dynamic openidConfiguration = JObject.Parse(await client.GetStringAsync(metadataAddress));
                string jwksUri = openidConfiguration.jwks_uri;
                dynamic jwks = JObject.Parse(await client.GetStringAsync(jwksUri));
                string certificate = jwks.keys[0].x5c[0];
                string issuer = openidConfiguration.issuer;
                string accessTokenIssuer = openidConfiguration.access_token_issuer;
                return new OpenIdConnectConfiguration
                {
                    Issuer = accessTokenIssuer ?? issuer,
                    Certificate = certificate
                };
            }
        }
    }
}