using System.Net.Http;
using System.Threading.Tasks;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    public static class OpenIdConnectConfigurationClient
    {
        public static Task<OpenIdConnectConfiguration> RequestConfigurationAsync(string metadataAddress)
        {
            /*
            One could use the following code, if only ADFS did not use the non standard OpenID Connect
            field access_token_issuer for the iss claim... :(
            The class Microsoft.IdentityModel.Protocols.OpenIdConnectConfiguration follows spec, and does
            not have functionality for retrieving the value of the access_token_issuer field.

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress);
            return configurationManager.GetConfigurationAsync();
            */

            return InternalConfigurationClient.RequestConfigurationAsync(metadataAddress, () => new InternalHttpClient());
        }

        private class InternalHttpClient : HttpClient, IHttpClient { }
    }
}
