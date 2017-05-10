using CarPool.Clients.Core.Models;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.User
{
    public interface IUserService
    {
        Task<Microsoft.Graph.User> GetUserByPrincipalNameAsync(string userPrincipalName);

        Task<Microsoft.Graph.User> GetUserByDisplayNameAsync(string displayName);

        Task<byte[]> GetUserPhotoAsync(string mail);

        Task<List<CarPool.Clients.Core.Models.Driver>> FilterMyDeparmentUsersAsync(string department, IEnumerable<CarPool.Clients.Core.Models.Driver> users);

        Task GetOutOfOfficeAsync(IEnumerable<GraphUser> users);

        Task<string> InitializeDeltaQueryAsync();

        Task<GraphServiceUsersCollectionResponse> GetDeltaQueryChangesAsync(string deltaLink);
    }
}