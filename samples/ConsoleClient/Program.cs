using System;
using System.ServiceModel;
using IdentityModel.Client;
using Wcf.Extensions.OpenIdConnect.Client;

namespace Wcf.Extensions.OpenIdConnect.Samples.ConsoleClient
{
    internal static class Program
    {
        // Uri to ConsoleHost
        //private const string ServiceUri = "https://localhost:44339/do-stuff.svc";
        // Uri to WebHost
        private const string ServiceUri = "https://localhost:44332/do-stuff.svc";

        private const string ClientId = "f87c1b06-b36a-470a-83a3-ca42f62915a8";
        private const string ClientSecret = "SsjRxUk3OBekcgysC7LVmvn9_48WZRVY3Z6APgHI";
        private const string TokenUri = "https://adfs.johan.local/adfs/oauth2/token";

        private static void Main()
        {
            Console.WriteLine("Ready:");
            Console.ReadLine();

            Console.Write("Client Credential Ping ");
            Console.WriteLine(InvokeWithClientCredentials());

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static string InvokeWithClientCredentials()
        {
            // With IdentityModel Token Client

            using (var client = new TokenClient(TokenUri, ClientId, ClientSecret))
            {
            // Request token
                var token = client.RequestClientCredentialsAsync("write").Result;
                // Create channel
                var channel = WrappedJwtChannelFactory.Create<IService>(token.AccessToken, ServiceUri);
            // Invoke proxy
            return channel.Ping();
        }

            // With Azure ADAL Token Client

            // Request token
            //var authContext = new AuthenticationContext(TokenUri);
            //var creds = new ClientCredential(ClientId, ClientSecret);
            //var token = authContext.AcquireTokenAsync(ResourceId, creds).Result;
            // Create channel
            //var channel = WrappedJwtChannelFactory.Create<IService>(token.AccessToken, ServiceUri);
            // Invoke proxy
            //return channel.Ping();
        }
    }

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Ping();
    }
}
