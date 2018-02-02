using System;
using Wcf.Extensions.OpenIdConnect.Host;
using Xunit;

namespace Wcf.Extensions.OpenIdConnect.Specs
{
    public class BadConfigurationTests
    {
        [Theory]
        [InlineData("", "certificate", "validAudience", "requiredScopes", "Issuer")]
        [InlineData("issuer", "", "validAudience", "requiredScopes", "Certificate")]
        [InlineData("issuer", "certificate", "", "requiredScopes", "validAudience")]
        [InlineData("issuer", "certificate", "validAudience", "", "requiredScopes")]
        public void WithEmptyConfig_ShouldThrow(string issuer, string certificate, string validAudience, string requiredScopes, string propertyName)
        {
            var testCode = CreateAction(issuer, certificate, validAudience, requiredScopes);
            var ex = Assert.Throws<OpenIdConnectConfigurationException>(testCode);
            Assert.Equal(propertyName, ex.PropertyName);
        }

        [Theory]
        [InlineData(null, "certificate", "validAudience", "requiredScopes", "Issuer")]
        [InlineData("issuer", null, "validAudience", "requiredScopes", "Certificate")]
        [InlineData("issuer", "certificate", null, "requiredScopes", "validAudience")]
        [InlineData("issuer", "certificate", "validAudience", null, "requiredScopes")]
        public void WithNullConfig_ShouldThrow(string issuer, string certificate, string validAudience, string requiredScopes, string paramName)
        {
            var testCode = CreateAction(issuer, certificate, validAudience, requiredScopes);
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.Equal(paramName, ex.ParamName);
        }

        private static Action CreateAction(string issuer, string certificate, string validAudience, string requiredScopes)
        {
            var configuration = new OpenIdConnectConfiguration
            {
                Issuer = issuer,
                Certificate = certificate
            };
            // ReSharper disable once ObjectCreationAsStatement - Test code
            return () => new WrappedJwtSecurityTokenHandler(configuration, validAudience, requiredScopes);
        }
    }
}
