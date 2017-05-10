using CarPool.Clients.Core.Models;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Organization
{
    public interface IOrganizationService
    {
        Task<string> GetOrganizationLocationAsync();

        Task SaveOrganizationLocationAsync();
    }
}