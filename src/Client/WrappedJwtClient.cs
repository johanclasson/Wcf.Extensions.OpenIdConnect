using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    public class WrappedJwtClient
    {
        private readonly string _tokenUri;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public WrappedJwtClient(string tokenUri, string clientId, string clientSecret)
        {
            _tokenUri = tokenUri;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<SecurityToken> RequestClientCredentialsAsync(string scope = "openid")
        {
            using (var oauth2Client = new TokenClient(_tokenUri, _clientId, _clientSecret))
            {
                var tokenResponse = await oauth2Client.RequestClientCredentialsAsync(scope);
                return WrapTokenResponse(tokenResponse);
            }
        }

        public async Task<SecurityToken> RequestResourceOwnerPasswordAsync(
            string userName, string password, string scope = "openid profile")
        {
            using (var oauth2Client = new TokenClient(_tokenUri, _clientId, _clientSecret))
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