using System.Threading.Tasks;
using Wcf.Extensions.OpenIdConnect.Host;
using Wcf.Extensions.OpenIdConnect.Specs.Support;
using Xunit;

namespace Wcf.Extensions.OpenIdConnect.Specs
{
    public class ConfigurationSpecs
    {
        private readonly ConfigurationFixture _fixture;

        public ConfigurationSpecs()
        {
            _fixture = new ConfigurationFixture();
            _fixture.GivenMetadataAddress("https://adfs.johan.local/adfs/.well-known/openid-configuration");
            _fixture.GivenHttpResponse("https://adfs.johan.local/adfs/discovery/keys", SpecConstants.ConfigurationKeys);
        }

        [Theory]
        [InlineData(SpecConstants.ConfigurationWithAccessTokenIssuer, "http://ADFS.johan.local/adfs/services/trust")]
        [InlineData(SpecConstants.ConfigurationWithoutAccessTokenIssuer, "https://ADFS.johan.local/adfs")]
        public async Task RequestedConfig_ShouldPickAccessTokenIssuerOverIssuer_ShouldGetCertificate(string configuration, string expectedIssuer)
        {
            _fixture.GivenHttpResponse("https://adfs.johan.local/adfs/.well-known/openid-configuration", configuration);
            OpenIdConnectConfiguration result = await _fixture.RequestConfigurationAsync();
            Assert.NotNull(result);
            Assert.Equal(expectedIssuer, result.Issuer);
            Assert.StartsWith("MIIC3DCCAcSgAwIBAgI", result.Certificate);
        }
    }
}
