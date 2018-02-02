using System;
using System.Threading.Tasks;

namespace Wcf.Extensions.OpenIdConnect.Host
{
    internal interface IHttpClient : IDisposable
    {
        Task<string> GetStringAsync(string requestUri);
    }
}