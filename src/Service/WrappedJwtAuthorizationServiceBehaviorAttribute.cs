using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    public class WrappedJwtAuthorizationServiceBehaviorAttribute : Attribute, IServiceBehavior
    {
        private readonly WrappedJwtAuthorizationServiceBehavior _behavior;

        public WrappedJwtAuthorizationServiceBehaviorAttribute(
            string validAudience = null,
            string validAudienceAppSettingKey = null,
            string metadataAddress = null,
            string metadataAddressAppSettingsKey = null,
            string requiredScopes = null)
        {
            _behavior = new WrappedJwtAuthorizationServiceBehavior(
                requiredScopes: requiredScopes,
                validAudience: validAudience,
                validAudienceAppSettingKey: validAudienceAppSettingKey,
                metadataAddress: metadataAddress,
                metadataAddressAppSettingsKey: metadataAddressAppSettingsKey);
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
