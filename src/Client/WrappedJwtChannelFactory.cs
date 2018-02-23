using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    // ReSharper disable once ClassNeverInstantiated.Global - Only used with static factory method in samples
    public static class WrappedJwtChannelFactory
    {
        public static TChannel CreateChannelWithIssuedToken<TChannel>(
            this ChannelFactory<TChannel> factory, string jwt, EndpointAddress address)
        {
            SecurityToken issuedToken = SamlUtils.WrapJwt(jwt);
            return factory.CreateChannelWithIssuedToken(issuedToken, address);
        }

        public static TChannel CreateChannelWithIssuedToken<TChannel>(
            this ChannelFactory<TChannel> factory, string jwt, EndpointAddress address, Uri via)
        {
            SecurityToken issuedToken = SamlUtils.WrapJwt(jwt);
            return factory.CreateChannelWithIssuedToken(issuedToken, address, via);
        }

        public static TChannel CreateChannelWithIssuedToken<TChannel>(
            this ChannelFactory<TChannel> factory, string jwt)
        {
            SecurityToken issuedToken = SamlUtils.WrapJwt(jwt);
            return factory.CreateChannelWithIssuedToken(issuedToken);
        }

        public static TChannel Create<TChannel>(string jwt, string remoteAddress)
        {
            var factory = new ChannelFactory<TChannel>(
                BindingFactory.ForWrappedJwt,
                new EndpointAddress(remoteAddress));
            return factory.CreateChannelWithIssuedToken(jwt);
        }
    }
}