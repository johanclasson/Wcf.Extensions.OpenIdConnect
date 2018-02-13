using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
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

            Console.Write("Resource Owner Ping ");
            Console.WriteLine(InvokeWithResourceOwnerPassword());

            Console.ReadLine();
        }

        private static string InvokeWithClientCredentials()
        {
            // Request token
            var client = new WrappedJwtClient(TokenUri, ClientId, ClientSecret);
            SecurityToken token = client.RequestClientCredentialsAsync(scope: "openid write").Result;
            // Create channel - With helper channel factory
            var channel = WrappedJwtChannelFactory.Create<IService>(token, ServiceUri);
            // Invoke proxy
            return channel.Ping();
        }

        private static string InvokeWithResourceOwnerPassword()
        {
            // Request token
            var client = new WrappedJwtClient(TokenUri, ClientId, ClientSecret);
            SecurityToken token = client.RequestResourceOwnerPasswordAsync(
                scope: "openid profile write", userName: "johan@johan.local", password: "!QAZ2wsx3edc").Result;
            // Create channel
            var factory = new ChannelFactory<IService>(
                BindingFactory.ForWrappedJwt,
                new EndpointAddress(ServiceUri));
            var channel = factory.CreateChannelWithIssuedToken(token);
            // Invoke proxy
            return channel.Ping();
        }
    }

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Ping();
    }
}
