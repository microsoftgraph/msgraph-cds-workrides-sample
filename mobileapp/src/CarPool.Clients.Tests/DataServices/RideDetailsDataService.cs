using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarPool.Clients.Tests.DataServices
{
    [TestFixture]
    public class RideDetailsDataService
    {
        private IDataService _dataService;

        private RideDetails _rideDetails;

        [SetUp]
        public void Init()
        {
            _dataService = new AzureMobileAppService();

            _rideDetails = new RideDetails
            {
                Driver = "JhonD",
                Date = DateTime.Now,
                Rider1 = "MeganB",
                Rider1Status = true,
                Distance = 140
            };
        }

        [TearDown]
        public async Task Close()
        {
            var rideDetails = (await _dataService.GetAllRideDetailsAsync())
                .FirstOrDefault(d => d.Driver.Equals(_rideDetails.Driver));

            if (rideDetails != null)
            {
                await _dataService.Remove<RideDetails>(rideDetails);
            }
        }

        [Test]
        public async Task TestRideDetailsLifecycle()
        {
            // INSERT rideDetails
            await _dataService.InsertAsync<RideDetails>(_rideDetails);

            // check rideDetails is stored
            _rideDetails = (await _dataService.GetAllRideDetailsAsync())
                .FirstOrDefault(d => d.Driver.Equals(_rideDetails.Driver));
            Assert.NotNull(_rideDetails, "RideDetails hasn't been created.");

            // UPDATE rideDetails
            _rideDetails.Rider1 = "JhonDoe";
            await _dataService.InsertOrUpdateRideDetailsAsync(_rideDetails);

            //check
            _rideDetails = (await _dataService.GetAllRideDetailsAsync())
                .FirstOrDefault(d => d.Rider1.Equals("JhonDoe"));
            Assert.NotNull(_rideDetails, "RideDetails hasn't been updated.");

            // REMOVE rideDetails
            await _dataService.Remove<RideDetails>(_rideDetails);

            // check rideDetails is removed
            var rideDetails = await _dataService.GetAllRideDetailsAsync();
            Assert.Null(rideDetails.FirstOrDefault(d => d.Driver.Equals(_rideDetails.Driver)), "RideDetails hasn't been removed.");
        }


    }
}
