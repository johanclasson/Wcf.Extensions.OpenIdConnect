using System;
using Wcf.Extensions.OpenIdConnect.Service;
using Xunit;

namespace Wcf.Extensions.OpenIdConnect.Specs
{
    public class BadTokenHandlerConfigurationSpecs
    {
        [Theory]
        [InlineData("", "certificate", "validAudience", "Issuer")]
        [InlineData("issuer", "", "validAudience", "Certificate")]
        [InlineData("issuer", "certificate", "", "validAudience")]
        public void WithEmptyConfig_ShouldThrow(
            string issuer, string certificate, string validAudience, string propertyName)
        {
            var testCode = CreateAction(
                issuer, certificate, validAudience, "", "");
            var ex = Assert.Throws<OpenIdConnectConfigurationException>(testCode);
            Assert.Equal(propertyName, ex.PropertyName);
        }

        [Theory]
        [InlineData(null, "certificate", "validAudience", "requiredScopes", "requiredRoles", "Issuer")]
        [InlineData("issuer", null, "validAudience", "requiredScopes", "requiredRoles", "Certificate")]
        [InlineData("issuer", "certificate", null, "requiredScopes", "requiredRoles", "validAudience")]
        [InlineData("issuer", "certificate", "validAudience", null, "requiredRoles", "requiredScopes")]
        [InlineData("issuer", "certificate", "validAudience", "requiredScopes", null, "requiredRoles")]
        public void WithNullConfig_ShouldThrow(
            string issuer, string certificate, string validAudience,
            string requiredScopes, string requiredRoles, string paramName)
        {
            var testCode = CreateAction(issuer, certificate, validAudience, requiredScopes, requiredRoles);
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.Equal(paramName, ex.ParamName);
        }

        private static Action CreateAction(
            string issuer, string certificate, string validAudience,
            string requiredScopes, string requiredRoles)
        {
            var configuration = new OpenIdConnectConfiguration
            {
                Issuer = issuer,
                Certificate = certificate
            };
            // ReSharper disable once ObjectCreationAsStatement - Test code
            return () => new WrappedJwtSecurityTokenHandler(
                configuration, validAudience, requiredScopes, requiredRoles);
        }
    }
}
