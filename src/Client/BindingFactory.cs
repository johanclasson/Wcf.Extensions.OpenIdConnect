using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    public static class BindingFactory
    {
        private static WS2007FederationHttpBinding _forClaims;

        public static WS2007FederationHttpBinding ForClaims => _forClaims ?? (_forClaims = CreateBindingForClaims());

        private static WS2007FederationHttpBinding CreateBindingForClaims()
        {
            var securityMode = WSFederationHttpSecurityMode.TransportWithMessageCredential;
            var binding = new WS2007FederationHttpBinding(securityMode)
            {
                //TODO: Is this really needed
                HostNameComparisonMode = HostNameComparisonMode.Exact
            };
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;
            return binding;
        }
    }
}