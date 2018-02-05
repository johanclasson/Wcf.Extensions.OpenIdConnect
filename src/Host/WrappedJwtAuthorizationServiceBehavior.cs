using System.Collections.ObjectModel;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    public class WrappedJwtAuthorizationServiceBehavior : IServiceBehavior
    {
        // ReSharper disable MemberCanBePrivate.Global - Property intended to be used in web.config
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global - Property intended to be used in web.config
        public string ValidAudienceAppSettingKey { get; set; }
        public string MetadataAddressKey { get; set; }
        public string RequiredScopes { get; set; }
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper restore MemberCanBePrivate.Global

        private static readonly object Locker = new object();
        private static OpenIdConnectConfiguration _configuration;

        public WrappedJwtAuthorizationServiceBehavior(
            string validAudienceAppSettingKey = Constants.DefaultValidAudienceAppSettingKey,
            string metadataAddressKey = Constants.DefaultMetadataAddressKey,
            string requiredScopes = "")
        {
            ValidAudienceAppSettingKey = validAudienceAppSettingKey;
            MetadataAddressKey = metadataAddressKey;
            RequiredScopes = requiredScopes;
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
            var config = GetConfig();
            var validAudience = GetValidAudience();
            serviceHostBase.AddWrappedJwtAuthorization(config, validAudience, RequiredScopes);
        }

        // TODO: Move app setting stuff to attribute only!?
        private string GetValidAudience() => GetAppSetting(ValidAudienceAppSettingKey);

        // TODO: Move app setting stuff to attribute only!?
        private string GetMetadataAddress() => GetAppSetting(MetadataAddressKey);

        private string GetAppSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new ApplicationConfigurationException(key);
            return value;
        }

        private OpenIdConnectConfiguration GetConfig()
        {
            lock (Locker)
            {
                return _configuration ?? (_configuration = OpenIdConnectConfigurationClient
                           .RequestConfigurationAsync(GetMetadataAddress()).Result);
            }
        }
    }
}