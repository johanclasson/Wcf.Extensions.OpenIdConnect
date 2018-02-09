using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    internal class InternalWrappedJwtAuthorizationServiceBehavior : IServiceBehavior
    {
        private Func<string, OpenIdConnectConfiguration> GetConfig { get; }
        private Func<string, string> GetSetting { get; }
        private readonly string _validAudience;
        private readonly string _metadataAddress;
        private readonly string _requiredScopes;

        public InternalWrappedJwtAuthorizationServiceBehavior(
            Func<string, OpenIdConnectConfiguration> getConfig,
            Func<string, string> getSetting,
            string validAudience = null,
            string validAudienceAppSettingKey = null,
            string metadataAddress = null,
            string metadataAddressAppSettingsKey = null,
            string requiredScopes = null)
        {
            GetConfig = getConfig;
            GetSetting = getSetting;
            _validAudience = GetOrDefaultFromSettings(
                validAudience, validAudienceAppSettingKey, Constants.DefaultValidAudienceAppSettingKey);
            _metadataAddress = GetOrDefaultFromSettings(
                metadataAddress, metadataAddressAppSettingsKey, Constants.DefaultMetadataAddressAppSettingsKey);
            _requiredScopes = GetOrDefault(requiredScopes, "");
        }

        private string GetOrDefault(string value, string defaultValue)
        {
            if (value == null)
                return defaultValue;
            return value;
        }

        private string GetOrDefaultFromSettings(string value, string key, string defaultKey)
        {
            var result = value;
            string keyOrDefault = GetOrDefault(key, defaultKey);
            if (result == null)
                result = GetSetting(keyOrDefault);
            if (string.IsNullOrEmpty(result))
                throw new ApplicationConfigurationException(keyOrDefault);
            return result;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // Calling init here works, but is really a hack. This method should really be used for inspection reasons only. See https://docs.microsoft.com/en-us/dotnet/api/system.servicemodel.description.iservicebehavior.validate?view=netframework-4.5 for details.
            Init(serviceHostBase);
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // This method is intended to be used for extending WCF, see https://docs.microsoft.com/en-us/dotnet/api/system.servicemodel.description.iservicebehavior.applydispatchbehavior?view=netframework-4.5, but calling init here does not work. Perhaps because it is called after the runtime is initialized?
        }

        private void Init(ServiceHostBase serviceHostBase)
        {
            OpenIdConnectConfiguration config = GetConfig(_metadataAddress);
            serviceHostBase.AddWrappedJwtAuthorization(config, _validAudience, _requiredScopes);
        }
    }
}
