using CarPool.Clients.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Driver
{
    public interface IDriverService
    {
        Task<IEnumerable<GraphUser>> GetMyRidersAsync(GraphUser user);

        Task<IEnumerable<GraphUser>> GetMyRiderRequestsAsync(GraphUser user);
    }
}