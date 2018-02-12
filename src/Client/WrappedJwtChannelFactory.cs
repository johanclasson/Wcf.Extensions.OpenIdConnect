using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    public static class WrappedJwtChannelFactory
    {
        public static TChannel Create<TChannel>(SecurityToken issuedToken, string uri)
        {
            var factory = new ChannelFactory<TChannel>(
                BindingFactory.ForWrappedJwt,
                new EndpointAddress(uri));
            return factory.CreateChannelWithIssuedToken(issuedToken);
        }
    }
}