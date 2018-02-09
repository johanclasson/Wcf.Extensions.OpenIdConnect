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
            field access_token_issuer... :(
            The class Microsoft.IdentityModel.Protocols.OpenIdConnectConfiguration follows spec, but
            when used together with ADFS, it is impossible to get hold of the value used for iss-claim
            in the issued tokens.

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress);
            return configurationManager.GetConfigurationAsync(); KB4057142?
            */

            return InternalConfigurationClient.RequestConfigurationAsync(metadataAddress, () => new InternalHttpClient());
        }

        private class InternalHttpClient : HttpClient, IHttpClient { }
    }
}
