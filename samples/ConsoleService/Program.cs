using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wcf.Extensions.OpenIdConnect.Samples.ConsoleService
{
    internal static class Program
    {
        public static void Main()
        {
            var host = new ServiceHost(
                typeof(Service), 
                new Uri("https://localhost:44339"));

            // Init option 1: Call extension method directly
            //var config = OpenIdConnectConfigurationClient.RequestConfigurationAsync(Constants.MetadataAddress).Result;
            //host.AddWrappedJwtAuthorization(config,
            //    requiredRoles: Constants.RequiredRoles,
            //    requiredScopes: Constants.RequiredScopes,
            //    validAudience: Constants.ValidAudience);

            // Init option 2: Add behavior by code
            //host.Description.Behaviors.Add(new WrappedJwtAuthorizationServiceBehavior(
            //    requiredScopes: Constants.RequiredScopes,
            //    requiredRoles: Constants.RequiredRoles,
            //    validAudience: Constants.ValidAudience,
            //    metadataAddress: Constants.MetadataAddress));

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
