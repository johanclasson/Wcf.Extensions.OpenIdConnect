using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.Xml;
using System.Xml.Linq;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    public static class SamlUtils
    {
        public static SecurityToken WrapJwt(string jwt)
        {
            var subject = new ClaimsIdentity("saml");
            subject.AddClaim(new Claim("jwt", jwt));

            var descriptor = new SecurityTokenDescriptor
            {
                TokenType = "access_token",
                TokenIssuerName = "urn:wrappedjwt",
                Subject = subject
            };

            var handler = new Saml2SecurityTokenHandler();
            SecurityToken token = handler.CreateToken(descriptor);
            var stringWriter = new StringWriter();
            handler.WriteToken(new XmlTextWriter(stringWriter), token);
            var tokenXml = stringWriter.ToString();
            var tokenXmlElement = XElement.Parse(tokenXml).ToXmlElement();

            return new GenericXmlSecurityToken(
                tokenXmlElement,
                null,
                DateTime.Now,
                // The jwt token already contains time of expire. No need to copy it into the wrapper token!
                DateTime.Now.AddHours(1),
                null,
                null,
                null);
        }
    }

    internal static class XElementExtensions
    {
        public static XmlElement ToXmlElement(this XElement el)
        {
            var doc = new XmlDocument();
            using (var reader = el.CreateReader())
            {
                doc.Load(reader);
                return doc.DocumentElement;
            }
        }
    }
}