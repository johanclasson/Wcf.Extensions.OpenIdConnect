using System.Collections.ObjectModel;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    public class WrappedJwtAuthorizationServiceBehavior : IServiceBehavior
    {
        private static OpenIdConnectConfiguration _configuration;
        private static readonly object Locker = new object();
        private readonly InternalWrappedJwtAuthorizationServiceBehavior _behavior;

        public WrappedJwtAuthorizationServiceBehavior(
            string validAudience = null,
            string validAudienceAppSettingKey = null,
            string metadataAddress = null,
            string metadataAddressAppSettingsKey = null,
            string requiredScopes = null)
        {
            _behavior = new InternalWrappedJwtAuthorizationServiceBehavior(
                validAudience: validAudience,
                validAudienceAppSettingKey: validAudienceAppSettingKey,
                metadataAddress: metadataAddress,
                metadataAddressAppSettingsKey: metadataAddressAppSettingsKey,
                requiredScopes: requiredScopes,
                getConfig: GetConfig,
                getSetting: GetSetting);
        }

        private OpenIdConnectConfiguration GetConfig(string metadataAddress)
        {
            lock (Locker)
            {
                return _configuration ?? (_configuration = OpenIdConnectConfigurationClient
                           .RequestConfigurationAsync(metadataAddress).Result);
            }
        }

        private string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            _behavior.Validate(serviceDescription, serviceHostBase);
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
            _behavior.AddBindingParameters(serviceDescription, serviceHostBase, endpoints, bindingParameters);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            _behavior.ApplyDispatchBehavior(serviceDescription, serviceHostBase);
        }
    }
}