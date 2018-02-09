using System;
using System.Threading.Tasks;

namespace Wcf.Extensions.OpenIdConnect.Service
{
    internal interface IHttpClient : IDisposable
    {
        Task<string> GetStringAsync(string requestUri);
    }
}
