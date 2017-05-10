using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarPool.Clients.Tests.DataServices
{
    [TestFixture]
    public class DriverDataService
    {
        private IDataService _dataService;

        private Driver _driver;

        private string _riderName;

        [SetUp]
        public void Init()
        {
            _dataService = new AzureMobileAppService();

            _driver = new Driver
            {
                Name = "JhonD",
                DisplayName = "Jhon Doe",
                Arrival = DateTime.Now,
                Departure = DateTime.Now,
            };

            _riderName = "MeganB";
        }

        [TearDown]
        public async Task Close()
        {
            var driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(_driver.Name));

            if (driver != null)
            {
                await _dataService.Remove<Driver>(_driver);
            }
        }

        [Test]
        public async Task TestDriverLifecycle()
        {
            // INSERT driver
            await _dataService.InsertAsync<Driver>(_driver);

            // check driver is stored
            _driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(_driver.Name));
            Assert.NotNull(_driver, "Driver hasn't been created.");
        
            // INSERT rider
            _driver.Rider1 = _riderName;
            await _dataService.InsertOrUpdateDriverAsync(_driver);

            //check
            _driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(_driver.Name));
            Assert.AreEqual(_driver.Rider1, _riderName, "Driver hasn't been updated.");
            Assert.AreEqual(false, _driver.Rider1Status, "Rider hasn't been created well.");
        
            // ACCEPT rider
            await _dataService.UpdateRiderStatusAsync(_driver, _riderName);

            //check
            _driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(_driver.Name));
            Assert.AreEqual(_driver.Rider1, _riderName, "Driver hasn't been accepted.");
            Assert.AreEqual(true, _driver.Rider1Status, "Driver hasn't been accepted.");
        
            // REJECT RIDER
            await _dataService.RejectRiderAsync(_driver, _riderName);

            // check
            _driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(_driver.Name));
            Assert.IsNull(_driver.Rider1, "Rider has'nt been rejected.");
            Assert.AreEqual(false, _driver.Rider1Status, "Rider hasn't been rejected.");
        
            // REMOVE user
            await _dataService.Remove<Driver>(_driver);

            // check driver is removed
            var drivers = await _dataService.GetAllDriversAsync();
            Assert.Null(drivers.FirstOrDefault(d => d.Name.Equals(_driver.Name)), "Driver hasn't been removed.");
        }


    }
}
