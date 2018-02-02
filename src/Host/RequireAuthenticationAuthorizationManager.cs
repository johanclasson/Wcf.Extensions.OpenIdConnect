using System.Security.Claims;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    internal class RequireAuthenticationAuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            return context.Principal.Identity.IsAuthenticated;
        }
    }
}
