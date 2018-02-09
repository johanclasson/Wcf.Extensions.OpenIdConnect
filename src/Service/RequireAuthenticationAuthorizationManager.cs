using System.Security.Claims;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    internal class RequireAuthenticationAuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            return context?.Principal?.Identity?.IsAuthenticated ?? false;
        }
    }
}
