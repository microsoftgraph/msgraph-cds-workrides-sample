using System.IO;
using System.Threading.Tasks;

namespace Carpool.Clients.Services.Data
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(string uri, string token);

        Task<byte[]> GetAsyncContent(string uri, string token = "");

        Task<TResult> PostAsync<TResult>(string uri, TResult data, string token);

        Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token);
    }
}
