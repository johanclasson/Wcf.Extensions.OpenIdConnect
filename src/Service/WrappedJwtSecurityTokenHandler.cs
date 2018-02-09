using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    internal class WrappedJwtSecurityTokenHandler : Saml2SecurityTokenHandler
    {
        readonly X509Certificate2 _signingCert;
        readonly string _issuerName;
        private readonly string _validAudience;
        private readonly string[] _requiredScopes;

        public WrappedJwtSecurityTokenHandler(OpenIdConnectConfiguration config, string validAudience, string requiredScopes)
        {
            Guard.AgainstNull(config, nameof(config));
            Guard.AgainstNullOrEmpty(config.Issuer, nameof(config.Issuer));
            Guard.AgainstNullOrEmpty(config.Certificate, nameof(config.Certificate));
            Guard.AgainstNullOrEmpty(validAudience, nameof(validAudience));
            Guard.AgainstNull(requiredScopes, nameof(requiredScopes));

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            _signingCert = new X509Certificate2(Convert.FromBase64String(config.Certificate));
            _issuerName = config.Issuer;
            _validAudience = validAudience;
            _requiredScopes = requiredScopes?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override ReadOnlyCollection<ClaimsIdentity> ValidateToken(SecurityToken token)
        {
            if (!(token is Saml2SecurityToken saml))
                throw new SecurityTokenValidationException("SAML token is not of the expected type.");
            var samlAttributeStatement = saml.Assertion.Statements.OfType<Saml2AttributeStatement>().FirstOrDefault();
            if (samlAttributeStatement == null)
                throw new SecurityTokenValidationException("SAML token did not contain the expected assertion statement.");
            var jwtAttribute = samlAttributeStatement.Attributes.SingleOrDefault(sa => sa.Name.Equals("jwt", StringComparison.OrdinalIgnoreCase));
            if (jwtAttribute == null)
                throw new SecurityTokenValidationException("SAML token assertion did not contain the expected jwt attribute.");
            var jwt = jwtAttribute.Values.Single();
            
            var parameters = new TokenValidationParameters
            {
                ValidAudience = _validAudience,
                ValidIssuer = _issuerName,
                IssuerSigningToken = new X509SecurityToken(_signingCert)
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(jwt, parameters, out _);
            ThrowIfInvalidScope(principal);
            return principal.Identities.Take(1).ToList().AsReadOnly();
        }

        private void ThrowIfInvalidScope(ClaimsPrincipal principal)
        {
            if (!_requiredScopes.Any())
                return;
            foreach (var scope in _requiredScopes)
            {
                if (principal.HasClaim(c => IsClaimContainingExpectedScope(c, scope)))
                    return;
            }
            throw new SecurityTokenValidationException("Insufficient Scope");
        }

        private static bool IsClaimContainingExpectedScope(Claim c, string scope)
        {
            return c.Type == "scp" && c.Value.Split(' ').Any(s => s == scope);
        }
    }
}
