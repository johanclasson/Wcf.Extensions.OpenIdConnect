using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    public class WrappedJwtAuthorizationServiceBehaviorAttribute : Attribute, IServiceBehavior
    {
        private readonly WrappedJwtAuthorizationServiceBehavior _behavior;

        public WrappedJwtAuthorizationServiceBehaviorAttribute(
            string validAudienceAppSettingKey = Constants.DefaultValidAudienceAppSettingKey,
            string metadataAddressKey = Constants.DefaultMetadataAddressKey,
            string requiredScopes = "")
        {
            _behavior = new WrappedJwtAuthorizationServiceBehavior(
                validAudienceAppSettingKey, metadataAddressKey, requiredScopes);
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
