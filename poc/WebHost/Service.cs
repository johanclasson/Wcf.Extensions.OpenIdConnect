using System.Security.Claims;
using System.ServiceModel;
using System.Text;

namespace Wcf.Extensions.OpenIdConnect.Poc.WebHost
{
    // ReSharper disable once UnusedMember.Global - Created through Service Host Factory
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
