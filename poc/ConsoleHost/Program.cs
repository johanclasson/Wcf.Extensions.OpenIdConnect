using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Wcf.Extensions.OpenIdConnect.Host;

namespace Wcf.Extensions.OpenIdConnect.Poc.ConsoleHost
{
    internal static class Program
    {
        public static void Main()
        {
            var host = new ServiceHost(
                typeof(Service), 
                new Uri("https://localhost:44335"));

            // Init option 1: Call extension method directly
            //var config = OpenIdConnectConfigurationClient.RequestConfigurationAsync(
            //    "https://adfs.johan.local/adfs/.well-known/openid-configuration").Result;
            //host.AddWrappedJwtAuthorization(config, "microsoft:identityserver:273a928b-8f15-461d-a039-0005ba3e8f1d", "write");

            // Init option 2: Add behavior by code or web.config
            //host.Description.Behaviors.Add(new WrappedJwtAuthorizationServiceBehavior(requiredScopes: "write"));

            host.AddServiceEndpoint(typeof(IService), CreateBinding(), "do-stuff");

            host.Open();

            Console.WriteLine("server running...");
            Console.ReadLine();

            host.Close();
        }

        private static Binding CreateBinding()
        {
            var binding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);

            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;

            return binding;
        }
    }

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