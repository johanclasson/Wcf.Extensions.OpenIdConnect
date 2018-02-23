using System;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    public class OpenIdConfigurationMissingFieldException : Exception
    {
        public string MissingField { get; }

        public OpenIdConfigurationMissingFieldException(string missingField) :
            base($"The field {missingField} is missing from the configuration.")
        {
            MissingField = missingField;
        }
    }
}