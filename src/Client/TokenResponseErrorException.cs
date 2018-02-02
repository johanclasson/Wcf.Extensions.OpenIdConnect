using System;

namespace Wcf.Extensions.OpenIdConnect.Client
{
    public class TokenResponseErrorException : Exception
    {
        public TokenResponseErrorException(string error) : base($"Token request resulted in an error: {error}")
        {
        }
    }
}