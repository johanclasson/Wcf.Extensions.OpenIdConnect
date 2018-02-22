using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    public static class WrappedJwtChannelFactory
    {
        public static TChannel Create<TChannel>(string jwt, string uri, WS2007FederationHttpBinding binding = null)
        {
            var factory = new ChannelFactory<TChannel>(
                binding ?? BindingFactory.ForWrappedJwt,
                new EndpointAddress(uri));
            SecurityToken issuedToken = SamlUtils.WrapJwt(jwt);
            return factory.CreateChannelWithIssuedToken(issuedToken);
        }
    }
}