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

        private static void Main()
        {
            var client = new WrappedJwtTokenClient(
                clientId: "f87c1b06-b36a-470a-83a3-ca42f62915a8",
                clientSecret: "SsjRxUk3OBekcgysC7LVmvn9_48WZRVY3Z6APgHI",
                address: "https://adfs.johan.local/adfs/oauth2/token");

            Console.WriteLine("Ready:");
            Console.ReadLine();

            InvokeWithClientCredentials(client);

            Console.ReadLine();

            InvokeWithResourceOwnerPassword(client);

            Console.ReadLine();
        }

        private static void InvokeWithClientCredentials(WrappedJwtTokenClient client)
        {
            SecurityToken token = client.RequestClientCredentialsAsync(scope: "openid write").Result;

            // Helper channel factory
            var channel = WrappedJwtChannelFactory.Create<IService>(token, ServiceUri);

            Console.Write("Client Credential Ping ");
            Console.WriteLine(channel.Ping());
        }

        private static void InvokeWithResourceOwnerPassword(WrappedJwtTokenClient client)
        {
            SecurityToken token = client.RequestResourceOwnerPasswordAsync(
                scope: "openid profile write", userName: "johan@johan.local", password: "!QAZ2wsx3edc").Result;

            var factory = new ChannelFactory<IService>(
                BindingFactory.ForClaims,
                new EndpointAddress(ServiceUri));
            var channel = factory.CreateChannelWithIssuedToken(token);

            Console.Write("Resource Owner Ping ");
            Console.WriteLine(channel.Ping());
        }
    }

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Ping();
    }
}
