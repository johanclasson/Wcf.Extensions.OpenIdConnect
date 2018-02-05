using System;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    public class ApplicationConfigurationException : Exception
    {
        public string Name { get; }

        public ApplicationConfigurationException(string name)
            : base($"The application setting {name} is not set or empty.")
        {
            Name = name;
        }
    }
}