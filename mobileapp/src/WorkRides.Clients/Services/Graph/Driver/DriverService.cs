using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Services.Graph.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Driver
{
    public class DriverService : IDriverService
    {
        private IUserService _userService;
        private IDataService _dataService;

        public DriverService(
            IUserService userService,
            IDataService dataService)
        {
            _userService = userService;
            _dataService = dataService;
        }

        public async Task<IEnumerable<GraphUser>> GetMyRiderRequestsAsync(GraphUser user)
        {
            var users = new List<GraphUser>();
            var drivers = await _dataService.GetAllDriversAsync();
            var driver = drivers.FirstOrDefault(d => d.Name.Equals(user.UserPrincipalName));

            if (driver != null)
            {
                if(!string.IsNullOrEmpty(driver.Rider1) && !driver.Rider1Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider1);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
                if (!string.IsNullOrEmpty(driver.Rider2) && !driver.Rider2Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider2);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
                if (!string.IsNullOrEmpty(driver.Rider3) && !driver.Rider3Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider3);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
                if (!string.IsNullOrEmpty(driver.Rider4) && !driver.Rider4Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider4);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
            }

            return users;
        }

        public async Task<IEnumerable<GraphUser>> GetMyRidersAsync(GraphUser user)
        {
            var users = new List<GraphUser>();
            var drivers = await _dataService.GetAllDriversAsync();
            var driver = drivers.FirstOrDefault(d => d.Name.Equals(user.UserPrincipalName));

            if (driver != null)
            {
                if (!string.IsNullOrEmpty(driver.Rider1) && driver.Rider1Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider1);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
                if (!string.IsNullOrEmpty(driver.Rider2) && driver.Rider2Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider2);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
                if (!string.IsNullOrEmpty(driver.Rider3) && driver.Rider3Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider3);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
                if (!string.IsNullOrEmpty(driver.Rider4) && driver.Rider4Status)
                {
                    var graphRider = await _userService.GetUserByDisplayNameAsync(driver.Rider4);
                    if (graphRider != null)
                    {
                        users.Add(new GraphUser(graphRider));
                    }
                }
            }

            return users;
        }
    }
}