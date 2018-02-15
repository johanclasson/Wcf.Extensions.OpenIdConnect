using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    /*
     * Copy of IdentityModel.Client.AuthenticationStyle just so that users of the 
     * WrappedJwtClient does not need to reference IdentityModel.dll.
     */
    public enum AuthenticationStyle
    {
        BasicAuthentication = 0,
        // ReSharper disable UnusedMember.Global - Copy from IdentityModel.Client
        PostValues = 1,
        Custom = 2
        // ReSharper restore UnusedMember.Global
    }

    public class WrappedJwtClient
    {
        private readonly Func<TokenClient> _createClient;

        public WrappedJwtClient(
            string tokenUri,
            string clientId = "",
            string clientSecret = "",
            HttpMessageHandler innerHttpMessageHandler = null,
            AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
        {
            if (innerHttpMessageHandler == null)
                innerHttpMessageHandler = new HttpClientHandler();
            _createClient = () => new TokenClient(tokenUri, clientId, clientSecret,
                innerHttpMessageHandler, (IdentityModel.Client.AuthenticationStyle)style);
        }

        public async Task<SecurityToken> RequestClientCredentialsAsync(string scope = "")
        {
            using (var oauth2Client = _createClient())
            {
                var tokenResponse = await oauth2Client.RequestClientCredentialsAsync(scope);
                return WrapTokenResponse(tokenResponse);
            }
        }

        public async Task<SecurityToken> RequestResourceOwnerPasswordAsync(
            string userName, string password, string scope = "")
        {
            using (var oauth2Client = _createClient())
            {
                var tokenResponse = await oauth2Client.RequestResourceOwnerPasswordAsync(userName, password, scope);
                return WrapTokenResponse(tokenResponse);
            }
        }

        private static SecurityToken WrapTokenResponse(TokenResponse tokenResponse)
        {
            if (tokenResponse.Error != null)
                throw new TokenResponseErrorException(tokenResponse.Error);
            return SamlUtils.WrapJwt(tokenResponse.AccessToken);
        }
    }
}