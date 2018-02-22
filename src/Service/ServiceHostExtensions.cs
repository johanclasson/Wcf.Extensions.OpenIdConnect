using System.IdentityModel.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    public static class ServiceHostExtensions
    {
        public static void AddWrappedJwtAuthorization(
            this ServiceHostBase host, OpenIdConnectConfiguration config,
            string validAudience, string requiredScopes = "", string requiredRoles = "")
        {
            host.Credentials.IdentityConfiguration = CreateIdentityConfiguration(
                config, validAudience, requiredScopes, requiredRoles);
            host.Credentials.UseIdentityConfiguration = true;

            var authorizationBehavior = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
            authorizationBehavior.PrincipalPermissionMode = PrincipalPermissionMode.Always;
        }

        private static IdentityConfiguration CreateIdentityConfiguration(
            OpenIdConnectConfiguration config, string validAudience, string requiredScopes, string requiredRoles)
        {
            var identityConfiguration = new IdentityConfiguration();
            identityConfiguration.SecurityTokenHandlers.Clear();
            identityConfiguration.SecurityTokenHandlers.Add(
                new WrappedJwtSecurityTokenHandler(config, validAudience, requiredScopes, requiredRoles));
            identityConfiguration.ClaimsAuthorizationManager = new RequireAuthenticationAuthorizationManager();
            return identityConfiguration;
        }
    }
}
