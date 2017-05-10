using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarPool.Clients.Tests.DataServices
{
    [TestFixture]
    public class EmployeeDataService
    {
        private IDataService _dataService;

        private Employee _employee;

        [SetUp]
        public void Init()
        {
            _dataService = new AzureMobileAppService();

            _employee = new Employee
            {
                Name = "JhonD",
                Email = "JhonD@carpool.onmicrosoft.com",
                WorkAddress = "Microsoft Westlake terry office, Seattle",
                HomeAddress = "18600 Union Hill Road, Redmond 98052",
                Arrival = DateTime.Now,
                Departure = DateTime.Now,
                Latitude = 47.4923372,
                Longitude = -122.2901927,
                WorkLatitude = 47.621421,
                WorkLongitude = -122.338221
            };
        }

        [TearDown]
        public async Task Close()
        {
            var employee = (await _dataService.GetAllEmployeesAsync())
                .FirstOrDefault(d => d.Name.Equals(_employee.Name));

            if (employee != null)
            {
                await _dataService.Remove<Employee>(employee);
            }
        }

        [Test]
        public async Task TestEmployeeLifecycle()
        {
            // INSERT employee
            await _dataService.InsertAsync<Employee>(_employee);

            // check employee is stored
            _employee = (await _dataService.GetAllEmployeesAsync())
                .FirstOrDefault(d => d.Name.Equals(_employee.Name));
            Assert.NotNull(_employee, "Employee hasn't been created.");

            // UPDATE employee
            _employee.HomeAddress = "3350 157th Ave N.E, Redmond 98052";
            await _dataService.InsertOrUpdateEmployeeAsync(_employee);

            //check
            _employee = (await _dataService.GetAllEmployeesAsync())
                .FirstOrDefault(d => d.HomeAddress.Equals("3350 157th Ave N.E, Redmond 98052"));
            Assert.NotNull(_employee, "Employee hasn't been updated.");

            // REMOVE employee
            await _dataService.Remove<Employee>(_employee);

            // check employee is removed
            var employees = await _dataService.GetAllEmployeesAsync();
            Assert.Null(employees.FirstOrDefault(d => d.Name.Equals(_employee.Name)), "Employee hasn't been removed.");
        }


    }
}
