using System;
using System.ServiceModel;
using Wcf.Extensions.OpenIdConnect.Host;

namespace Wcf.Extensions.OpenIdConnect.Specs.Support
{
    internal static class SpecExtensions
    {
        public static void Init(this InternalWrappedJwtAuthorizationServiceBehavior sut)
        {
            sut.Validate(null, new ServiceHost(typeof(Service), new Uri("https://localhost:44341")));
        }

        [ServiceContract]
        private interface IService
        {
            [OperationContract]
            void Ping();
        }

        private class Service : IService
        {
            public void Ping()
            {
            }
        }
    }
}