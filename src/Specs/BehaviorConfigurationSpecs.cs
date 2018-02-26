using Wcf.Extensions.OpenIdConnect.Service;
using Wcf.Extensions.OpenIdConnect.Specs.Support;
using Xunit;

namespace Wcf.Extensions.OpenIdConnect.Specs
{
    public class BehaviorConfigurationSpecs
    {
        private readonly BehaviorConfigurationFixture _f;
        private const string SomeAudience = "fakeAudience";
        private const string SomeMetadataAddress = "fakeAddress";

        public BehaviorConfigurationSpecs()
        {
            _f = new BehaviorConfigurationFixture();
        }

        [Fact]
        public void GivenNoKeysOrConfig_ShouldRetrieveTheExpectedData()
        {
            _f.GivenFakeConfig(SomeMetadataAddress);
            _f.GivenFakeSetting("oid:MetadataAddress", SomeMetadataAddress);
            _f.GivenFakeSetting("oid:ValidAudience", SomeAudience);
            _f.CreateSut().Init();
            _f.ShouldHaveRetrievedAllFakedData();
        }

        [Fact]
        public void GivenAllConfig_ShouldRetrieveTheExpectedData()
        {
            _f.GivenFakeConfig(SomeMetadataAddress);
            _f.CreateSut(metadataAddress: SomeMetadataAddress, validAudience: SomeAudience).Init();
            _f.ShouldHaveRetrievedAllFakedData();
        }

        [Fact]
        public void GivenCustomKeys_ShouldRetrieveTheExpectedData()
        {
            _f.GivenFakeConfig(SomeMetadataAddress);
            _f.GivenFakeSetting("m1", SomeMetadataAddress);
            _f.GivenFakeSetting("v1", SomeAudience);
            _f.CreateSut(validAudienceAppSettingKey: "v1", metadataAddressAppSettingsKey: "m1").Init();
            _f.ShouldHaveRetrievedAllFakedData();
        }

        [Fact]
        public void GivenNoValidAudienceSetting_ShouldThrow()
        {
            // Example of no defined app setting
            var ex = Assert.Throws<ApplicationConfigurationException>(
                () => _f.CreateSut(metadataAddress: SomeMetadataAddress).Init());
            Assert.Equal("oid:ValidAudience", ex.AppSettingKey);
        }

        [Fact]
        public void GivenNoValidAddressSetting_ShouldThrow()
        {
            // Example of invalid app setting
            _f.GivenFakeSetting("oid:MetadataAddress", "");
            var ex = Assert.Throws<ApplicationConfigurationException>(
                () => _f.CreateSut(validAudience: SomeAudience).Init());
            Assert.Equal("oid:MetadataAddress", ex.AppSettingKey);
            _f.ShouldHaveRetrievedAllFakedData();
        }
    }
}
