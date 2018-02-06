using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wcf.Extensions.OpenIdConnect.Poc.ConsoleHost
{
    internal static class Program
    {
        public static void Main()
        {
            var host = new ServiceHost(
                typeof(Service), 
                new Uri("https://localhost:44339"));

            // Init option 1: Call extension method directly
            //var config = OpenIdConnectConfigurationClient.RequestConfigurationAsync(
            //    "https://adfs.johan.local/adfs/.well-known/openid-configuration").Result;
            //host.AddWrappedJwtAuthorization(config, "microsoft:identityserver:273a928b-8f15-461d-a039-0005ba3e8f1d", "write");

            // Init option 2: Add behavior by code
            //host.Description.Behaviors.Add(new WrappedJwtAuthorizationServiceBehavior(requiredScopes: "write"));

            host.AddServiceEndpoint(typeof(IService), CreateBinding(), "do-stuff.svc");

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
}