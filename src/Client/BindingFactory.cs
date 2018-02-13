using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    public static class BindingFactory
    {
        private static WS2007FederationHttpBinding _forWrappedJwt;

        public static WS2007FederationHttpBinding ForWrappedJwt => _forWrappedJwt ??
                                                                   (_forWrappedJwt = CreateForWrappedJwt());

        private static WS2007FederationHttpBinding CreateForWrappedJwt()
        {
            var binding = new WS2007FederationHttpBinding(
                WSFederationHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;
            return binding;
        }
    }
}