using System;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    public class OpenIdConnectConfigurationException : Exception
    {
        public string PropertyName { get; }

        public OpenIdConnectConfigurationException(string propertyName)
            : base($"The configuration property {propertyName} is empty.")
        {
            PropertyName = propertyName;
        }
    }
}
