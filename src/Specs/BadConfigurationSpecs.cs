using System.Threading.Tasks;
using Wcf.Extensions.OpenIdConnect.Service;
using Wcf.Extensions.OpenIdConnect.Specs.Support;
using Xunit;

namespace Wcf.Extensions.OpenIdConnect.Specs
{
    public class BadConfigurationSpecs
    {
        private readonly ConfigurationFixture _fixture;

        public BadConfigurationSpecs()
        {
            _fixture = new ConfigurationFixture();
            _fixture.GivenMetadataAddress("https://adfs.johan.local/adfs/.well-known/openid-configuration");
        }

        [Theory]
        [InlineData(SpecConstants.ConfigurationWithAccessTokenIssuer, SpecConstants.ConfigurationKeysWithoutX5CCertificate, "x5c")]
        [InlineData(SpecConstants.ConfigurationWithAccessTokenIssuer, SpecConstants.ConfigurationKeysWithoutX5CProperty, "x5c")]
        [InlineData(SpecConstants.ConfigurationWithAccessTokenIssuer, SpecConstants.ConfigurationKeysWithoutKeys, "keys")]
        [InlineData(SpecConstants.ConfigurationWithAccessTokenIssuer, SpecConstants.ConfigurationKeysWithoutKeyProperty, "keys")]
        [InlineData(SpecConstants.ConfigurationWithoutJwksUri, SpecConstants.ConfigurationKeys, "jwks_uri")]
        public async Task RequestedConfig_ShouldPickAccessTokenIssuerOverIssuer_ShouldGetCertificate(
            string configuration, string configurationKeys, string expectedMissingField)
        {
            _fixture.GivenHttpResponse("https://adfs.johan.local/adfs/.well-known/openid-configuration", configuration);
            _fixture.GivenHttpResponse("https://adfs.johan.local/adfs/discovery/keys", configurationKeys);
            Task Func() => _fixture.RequestConfigurationAsync();
            var ex = await Assert.ThrowsAsync<OpenIdConfigurationMissingFieldException>(Func);
            Assert.Equal(expectedMissingField, ex.MissingField);
        }
    }
}