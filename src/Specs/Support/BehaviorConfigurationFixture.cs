using System.Collections.Generic;
using System.Linq;
using Wcf.Extensions.OpenIdConnect.Host;
using Xunit;

namespace Wcf.Extensions.OpenIdConnect.Specs.Support
{
    internal class BehaviorConfigurationFixture
    {
        private readonly Dictionary<string, OpenIdConnectConfiguration> _configurations =
            new Dictionary<string, OpenIdConnectConfiguration>();

        private readonly List<string> _retrievedConfigUris = new List<string>();
        private readonly List<string> _retrievedSettingKeys = new List<string>();

        private readonly Dictionary<string, string> _settings =
            new Dictionary<string, string>();

        public void GivenFakeConfig(string uri)
        {
            _configurations[uri] = new OpenIdConnectConfiguration
            {
                Certificate = SpecConstants.Certificate,
                Issuer = "issuer1"
            };
        }

        public InternalWrappedJwtAuthorizationServiceBehavior CreateSut(
            string requiredScopes = null,
            string validAudience = null,
            string validAudienceAppSettingKey = null,
            string metadataAddress = null,
            string metadataAddressAppSettingsKey = null)
        {
            return new InternalWrappedJwtAuthorizationServiceBehavior(
                GetConfig,
                GetSetting,
                validAudience,
                validAudienceAppSettingKey,
                metadataAddress,
                metadataAddressAppSettingsKey,
                requiredScopes);
        }

        private string GetSetting(string key)
        {
            _retrievedSettingKeys.Add(key);
            if (_settings.ContainsKey(key))
                return _settings[key];
            return "";
        }

        private OpenIdConnectConfiguration GetConfig(string uri)
        {
            Assert.Contains(uri, _configurations.Keys);
            _retrievedConfigUris.Add(uri);
            return _configurations[uri];
        }

        public void GivenFakeSetting(string key, string value)
        {
            _settings[key] = value;
        }

        public void ShouldHaveRetrievedAllFakedData()
        {
            Assert.Equal(_configurations.Keys.OrderBy(k => k), _retrievedConfigUris.OrderBy(k => k));
            Assert.Equal(_settings.Keys.OrderBy(k => k), _retrievedSettingKeys.OrderBy(k => k));
        }
    }
}