using System.Collections.Generic;
using System.Threading.Tasks;
using Wcf.Extensions.OpenIdConnect.Host;

namespace Wcf.Extensions.OpenIdConnect.Specs.Support
{
    public class ConfigurationFixture : IHttpClient
    {
        private string _metadataAddress;
        private readonly Dictionary<string, string> _httpResponses = new Dictionary<string, string>();

        public void GivenHttpResponse(string address, string content)
        {
            _httpResponses[address] = content;
        }

        public void GivenMetadataAddress(string metadataAddress)
        {
            _metadataAddress = metadataAddress;
        }

        public Task<OpenIdConnectConfiguration> RequestConfigurationAsync()
        {
            return InternalConfigurationClient.RequestConfigurationAsync(_metadataAddress, () => this);
        }

        public void Dispose()
        {
        }

        public Task<string> GetStringAsync(string requestUri)
        {
            return Task.FromResult(_httpResponses[requestUri]);
        }
    }
}