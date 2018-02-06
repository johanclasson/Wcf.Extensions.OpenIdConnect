using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using Wcf.Extensions.OpenIdConnect.Host;

namespace Wcf.Extensions.OpenIdConnect.Poc.ConsoleHost
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Ping();
    }

    // Init option 3: Add behavior by service attribute
    [WrappedJwtAuthorizationServiceBehavior(requiredScopes: "write")]
    internal class Service : IService
    {
        public string Ping()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Pong");

            foreach (var claim in ClaimsPrincipal.Current.Claims)
            {
                sb.AppendFormat("{0} :: {1}\n", claim.Type, claim.Value);
            }

            return sb.ToString().Trim();
        }
    }
}
