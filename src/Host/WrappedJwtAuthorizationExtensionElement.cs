﻿using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    public class WrappedJwtAuthorizationExtensionElement : BehaviorExtensionElement
    {
        private const string ValidAudienceName = "validAudience";
        private const string ValidAudienceAppSettingKeyName = "validAudienceAppSettingKey";
        private const string MetadataAddressName = "metadataAddress";
        private const string MetadataAddressAppSettingKeyName = "metadataAddressAppSettingKey";
        private const string RequiredScopesName = "requiredScopes";

        [ConfigurationProperty(ValidAudienceName)]
        private string ValidAudience => GetPropertyValue(ValidAudienceName);

        [ConfigurationProperty(ValidAudienceAppSettingKeyName)]
        private string ValidAudienceAppSettingKey => GetPropertyValue(ValidAudienceAppSettingKeyName);

        [ConfigurationProperty(MetadataAddressAppSettingKeyName)]
        private string MetadataAddressAppSettingKey => GetPropertyValue(MetadataAddressAppSettingKeyName);

        [ConfigurationProperty(MetadataAddressName)]
        private string MetadataAddress => GetPropertyValue(MetadataAddressName);

        [ConfigurationProperty(RequiredScopesName)]
        private string RequiredScopes => GetPropertyValue(RequiredScopesName);

        private string GetPropertyValue(string name)
        {
            var value = (string) base[name];
            if (string.IsNullOrEmpty(value))
                return null;
            return value;
        }

        protected override object CreateBehavior()
        {
            return new WrappedJwtAuthorizationServiceBehavior(
                requiredScopes: RequiredScopes,
                validAudience: ValidAudience,
                validAudienceAppSettingKey: ValidAudienceAppSettingKey,
                metadataAddress: MetadataAddress,
                metadataAddressAppSettingsKey: MetadataAddressAppSettingKey);
        }

        public override Type BehaviorType => typeof(WrappedJwtAuthorizationServiceBehavior);
    }
}