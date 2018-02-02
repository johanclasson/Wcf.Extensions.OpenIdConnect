using System;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    internal static class Guard
    {
        public static void AgainstNull(object obj, string propertyName)
        {
            if (obj != null)
                return;
            throw new ArgumentNullException(propertyName);
        }

        public static void AgainstNullOrEmpty(string obj, string propertyName)
        {
            AgainstNull(obj, propertyName);
            if (string.IsNullOrWhiteSpace(obj))
                throw new OpenIdConnectConfigurationException(propertyName);
        }

    }
}