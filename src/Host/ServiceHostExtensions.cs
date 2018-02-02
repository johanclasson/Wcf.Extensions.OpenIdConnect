using System.IdentityModel.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    public static class ServiceHostExtensions
    {
        public static void AddWrappedJwtAuthorization(this ServiceHostBase host, OpenIdConnectConfiguration config, string validAudience, string requiredScopes)
        {
            host.Credentials.IdentityConfiguration = CreateIdentityConfiguration(config, validAudience, requiredScopes);
            host.Credentials.UseIdentityConfiguration = true;

            var authorizationBehavior = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
            authorizationBehavior.PrincipalPermissionMode = PrincipalPermissionMode.Always;
        }

        private static IdentityConfiguration CreateIdentityConfiguration(OpenIdConnectConfiguration config, string validAudience, string requiredScopes)
        {
            var identityConfiguration = new IdentityConfiguration();
            identityConfiguration.SecurityTokenHandlers.Clear();
            identityConfiguration.SecurityTokenHandlers.Add(new WrappedJwtSecurityTokenHandler(config, validAudience, requiredScopes));
            identityConfiguration.ClaimsAuthorizationManager = new RequireAuthenticationAuthorizationManager();
            return identityConfiguration;
        }
    }
}
