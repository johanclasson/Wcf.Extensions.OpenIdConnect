using System;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    public class ApplicationConfigurationException : Exception
    {
        public string AppSettingKey { get; }

        public ApplicationConfigurationException(string appSettingKey)
            : base($"The application setting {appSettingKey} is not set or empty.")
        {
            AppSettingKey = appSettingKey;
        }
    }
}