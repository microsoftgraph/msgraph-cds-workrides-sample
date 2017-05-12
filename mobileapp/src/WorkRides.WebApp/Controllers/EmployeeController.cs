using CarPool.WebApp.Models;
using Microsoft.CommonDataService;
using Microsoft.CommonDataService.Builders;
using Microsoft.CommonDataService.Configuration;
using Microsoft.CommonDataService.ServiceClient.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;

namespace CarPool.WebApp.Controllers
{
    /// <summary>
    /// Controller for Employee
    /// This controller uses the default routing table constructed using method signature
    /// Learn more here: https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    /// </summary>
    public class EmployeeController : ApiController
    {
        // GET: api/Employee
        public async Task<IEnumerable<Employee>> Get()
        {
            //Get CDS SDK reference ready to communicate with CDS environment
            using (Client client = await CdsHelper.CreateClientAsync(this.Request))
            {
                //Build query
                DataRangeSkipClauseBuilder<CarPool.Web.Library.Employee> query = client.GetRelationalEntitySet<CarPool.Web.Library.Employee>()
                           .CreateQueryBuilder()
                           .Project(pc => pc
                                            .SelectField(f => f.PrimaryId)
                                            .SelectField(f => f.FullName)
                                            .SelectField(f => f.HomeAddress)
                                            .SelectField(f => f.WorkAddress)
                                            .SelectField(f => f.PreferredArrivalTimeAtWork)
                                            .SelectField(f => f.PreferredDepartureTimeFromWork)
                                            .SelectField(f => f.Email)
                                            .SelectField(f => f.Phone)
                                            .SelectField(f => f.BusinessUnit)
                                            .SelectField(f => f.HomeLatitude)
                                            .SelectField(f => f.HomeLongitude)
                                            .SelectField(f => f.WorkLatitude)
                                            .SelectField(f => f.WorkLongitude)
                                        );

                //Execute query
                OperationResult<IReadOnlyList<CarPool.Web.Library.Employee>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                //Transform query results
                var employees = new List<Employee>();
                foreach (var entry in queryResult.Result)
                {
                    employees.Add(CDSEmployeeToAppEmployee(entry));
                }
                return employees;
            }
        }

        // GET: api/Employee/5
        public async Task<Employee> Get(string id)
        {
            using (Client client = await CdsHelper.CreateClientAsync(this.Request))
            {
                DataRangeSkipClauseBuilder<CarPool.Web.Library.Employee> query = client.GetRelationalEntitySet<CarPool.Web.Library.Employee>()
               .CreateQueryBuilder()
               .Where(pc => pc.PrimaryId == id)
               .Project(pc => pc
                                .SelectField(f => f.PrimaryId)
                                .SelectField(f => f.FullName)
                                .SelectField(f => f.HomeAddress)
                                .SelectField(f => f.WorkAddress)
                                .SelectField(f => f.PreferredArrivalTimeAtWork)
                                .SelectField(f => f.PreferredDepartureTimeFromWork)
                                .SelectField(f => f.Email)
                                .SelectField(f => f.Phone)
                                .SelectField(f => f.BusinessUnit)
                                .SelectField(f => f.HomeLatitude)
                                .SelectField(f => f.HomeLongitude)
                                .SelectField(f => f.WorkLatitude)
                                .SelectField(f => f.WorkLongitude)
                            );

                OperationResult<IReadOnlyList<CarPool.Web.Library.Employee>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var employee = CDSEmployeeToAppEmployee(queryResult.Result.First());
                return employee;
            }
        }

        // POST: api/Employee
        public async Task Post([FromBody]Employee value)
        {                    
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var cdsEmployee = AppEmployeeToCDSEmployee(value);
                
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Insert(cdsEmployee)
                    .ExecuteAsync();
            }
        }

        // PUT: api/Employee/5
        public async Task Put(string id, [FromBody]Employee value)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var query = client.GetRelationalEntitySet<CarPool.Web.Library.Employee>()
                           .CreateQueryBuilder()
                           .Where(pc => pc.PrimaryId == id)
                           .Project(pc => pc
                                            .SelectField(f => f.PrimaryId)
                                            .SelectField(f => f.FullName)
                                            .SelectField(f => f.HomeAddress)
                                            .SelectField(f => f.WorkAddress)
                                            .SelectField(f => f.PreferredArrivalTimeAtWork)
                                            .SelectField(f => f.PreferredDepartureTimeFromWork)
                                            .SelectField(f => f.Email)
                                            .SelectField(f => f.Phone)
                                            .SelectField(f => f.BusinessUnit)
                                            .SelectField(f => f.HomeLatitude)
                                            .SelectField(f => f.HomeLongitude)
                                            .SelectField(f => f.WorkLatitude)
                                            .SelectField(f => f.WorkLongitude)
                                    );

                OperationResult<IReadOnlyList<CarPool.Web.Library.Employee>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var updateExecutor = client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional);
                var cdsEmployee = queryResult.Result.First();
                var updateEmployee = client.CreateRelationalFieldUpdates<CarPool.Web.Library.Employee>();

                updateEmployee.Update(pc => pc.FullName, value.Name);
                updateEmployee.Update(pc => pc.HomeAddress, CreateNewAddressWithLine1(value.HomeAddress));
                updateEmployee.Update(pc => pc.WorkAddress, CreateNewAddressWithLine1(value.WorkAddress));
                updateEmployee.Update(pc => pc.PreferredArrivalTimeAtWork, new DateTimeOffset(value.Arrival.ToUniversalTime()));
                updateEmployee.Update(pc => pc.PreferredDepartureTimeFromWork, new DateTimeOffset(value.Departure.ToUniversalTime()));
                updateEmployee.Update(pc => pc.Email, value.Email);
                updateEmployee.Update(pc => pc.Phone, value.PhoneNo);
                //Not used in app -- updateEmployee.Update(pc => pc.BusinessUnit, value.BusinessUnit);
                updateEmployee.Update(pc => pc.HomeLatitude, Convert.ToDecimal(value.Latitude));
                updateEmployee.Update(pc => pc.HomeLongitude, Convert.ToDecimal(value.Longitude));
                updateEmployee.Update(pc => pc.WorkLatitude, Convert.ToDecimal(value.WorkLatitude));
                updateEmployee.Update(pc => pc.WorkLongitude, Convert.ToDecimal(value.WorkLongitude));

                updateExecutor.Update(cdsEmployee, updateEmployee);
                await updateExecutor.ExecuteAsync();
            }
        }

        // DELETE: api/Employee/5
        public async Task Delete(string id)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var query = client.GetRelationalEntitySet<CarPool.Web.Library.Employee>()
                           .CreateQueryBuilder()
                           .Where(pc => pc.PrimaryId == id)
                           .Project(pc => pc.SelectField(f => f.PrimaryId).SelectField(f => f.FullName));

                OperationResult<IReadOnlyList<CarPool.Web.Library.Employee>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var deleteExecutor = client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional);
                foreach (var entry in queryResult.Result)
                {
                    deleteExecutor.DeleteWithoutConcurrencyCheck(entry);
                }
                await deleteExecutor.ExecuteAsync();
            }
        }

        private Employee CDSEmployeeToAppEmployee(CarPool.Web.Library.Employee cdsEmployee)
        {
            Employee employee = new Employee();
            employee.Id = cdsEmployee.PrimaryId;
            employee.Name = cdsEmployee.FullName;
            employee.Email = cdsEmployee.Email;
            employee.PhoneNo = cdsEmployee.Phone;
            employee.WorkAddress = cdsEmployee.WorkAddress != null ? cdsEmployee.WorkAddress.Line1 : String.Empty;
            employee.HomeAddress = cdsEmployee.HomeAddress != null ? cdsEmployee.HomeAddress.Line1 : String.Empty;
            employee.Arrival = cdsEmployee.PreferredArrivalTimeAtWork.HasValue ? cdsEmployee.PreferredArrivalTimeAtWork.Value.ToDateTime().ToLocalTime() : new DateTime();
            employee.Departure = cdsEmployee.PreferredDepartureTimeFromWork.HasValue ? cdsEmployee.PreferredDepartureTimeFromWork.Value.ToDateTime().ToLocalTime() : new DateTime();
            employee.Latitude = cdsEmployee.HomeLatitude.HasValue ? Convert.ToDouble(cdsEmployee.HomeLatitude) : 0;
            employee.Longitude = cdsEmployee.HomeLongitude.HasValue ? Convert.ToDouble(cdsEmployee.HomeLongitude) : 0;
            employee.WorkLatitude = cdsEmployee.WorkLatitude.HasValue ? Convert.ToDouble(cdsEmployee.WorkLatitude) : 0;
            employee.WorkLongitude = cdsEmployee.WorkLongitude.HasValue ? Convert.ToDouble(cdsEmployee.WorkLongitude) : 0;

            return employee;
        }

        private CarPool.Web.Library.Employee AppEmployeeToCDSEmployee(Employee employee)
        {
            CarPool.Web.Library.Employee cdsEmployee = new CarPool.Web.Library.Employee();

            cdsEmployee.PrimaryId = employee.Id;
            UpdateCDSEmployeeFromAppEmployee(cdsEmployee, employee);

            return cdsEmployee;
        }

        private void UpdateCDSEmployeeFromAppEmployee(CarPool.Web.Library.Employee cdsEmployee, Employee employee)
        {
            //Could check for id match before update
            //cdsEmployee.PrimaryId = employee.Id;

            cdsEmployee.FullName = employee.Name;
            cdsEmployee.Email = employee.Email;
            cdsEmployee.Phone = employee.PhoneNo;
            if (cdsEmployee.WorkAddress != null)
            {
                cdsEmployee.WorkAddress.Line1 = employee.WorkAddress;
            }
            else
            {
                cdsEmployee.WorkAddress = CreateNewAddressWithLine1(employee.WorkAddress);
            }
            if (cdsEmployee.HomeAddress != null)
            {
                cdsEmployee.HomeAddress.Line1 = employee.HomeAddress;
            }
            else
            {
                cdsEmployee.HomeAddress = CreateNewAddressWithLine1(employee.HomeAddress);
            }
            cdsEmployee.PreferredArrivalTimeAtWork = new DateTimeOffset(employee.Arrival.ToUniversalTime());
            cdsEmployee.PreferredDepartureTimeFromWork = new DateTimeOffset(employee.Departure.ToUniversalTime());
            cdsEmployee.HomeLatitude = Convert.ToDecimal(employee.Latitude);
            cdsEmployee.HomeLongitude = Convert.ToDecimal(employee.Longitude);
            cdsEmployee.WorkLatitude = Convert.ToDecimal(employee.WorkLatitude);
            cdsEmployee.WorkLongitude = Convert.ToDecimal(employee.WorkLongitude);
        }

        private Address CreateNewAddressWithLine1(string line1)
        {
            Address address = new Address();
            address.Line1 = line1;
            address.Json = new Json();
            address.Json.Text = "";
            return address;
        }
    }


}
