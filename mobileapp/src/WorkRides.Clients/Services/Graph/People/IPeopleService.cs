using CarPool.Clients.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.People
{
    public interface IPeopleService
    {
        Task<List<CarPool.Clients.Core.Models.Driver>> FilterMyNearPeople(IEnumerable<CarPool.Clients.Core.Models.Driver> users);
    }
}