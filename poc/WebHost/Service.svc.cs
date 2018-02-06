using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using Wcf.Extensions.OpenIdConnect.Host;

namespace Wcf.Extensions.OpenIdConnect.Poc.WebHost
{
    [WrappedJwtAuthorizationServiceBehavior(requiredScopes: "write")]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Service : IService
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

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Ping();
    }
}
