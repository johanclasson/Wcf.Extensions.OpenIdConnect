using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    internal static class InternalConfigurationClient
    {
        public static async Task<OpenIdConnectConfiguration> RequestConfigurationAsync(string metadataAddress, Func<IHttpClient> createClient)
        {
            using (var client = createClient())
            {
                var json = await client.GetStringAsync(metadataAddress);
                dynamic openidConfiguration = JObject.Parse(json);
                string jwksUri = openidConfiguration.jwks_uri;
                dynamic jwks = JObject.Parse(await client.GetStringAsync(jwksUri));
                //TODO: Throw if no key or x5c is present
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
