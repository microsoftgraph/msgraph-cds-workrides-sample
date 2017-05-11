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
    /// Controller for Driver
    /// This controller uses the default routing table constructed using method signature
    /// Learn more here: https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    /// </summary>
    public class DriverController : ApiController
    {
        // GET: api/Driver
        public async Task<IEnumerable<Driver>> Get()
        {
            using (Client client = await CdsHelper.CreateClientAsync(this.Request))
            {
                DataRangeSkipClauseBuilder<CarPool.Web.Library.Driver> query = client.GetRelationalEntitySet<CarPool.Web.Library.Driver>()
                           .CreateQueryBuilder()
                           .Project(pc => pc
                                            .SelectField(f => f.PrimaryId)
                                            .SelectField(f => f.RouteTitle)
                                            .SelectField(f => f.AverageMiles)
                                            .SelectField(f => f.RatePerMile)
                                            .SelectField(f => f.Name)
                                            .SelectField(f => f.DisplayName)
                                            .SelectField(f => f.Schedule)
                                            .SelectField(f => f.HomeLatitude)
                                            .SelectField(f => f.HomeLongitude)
                                            .SelectField(f => f.Arrival)
                                            .SelectField(f => f.Departure)
                                            .SelectField(f => f.Rider1Name)
                                            .SelectField(f => f.Rider1Status)
                                            .SelectField(f => f.Rider2Name)
                                            .SelectField(f => f.Rider2Status)
                                            .SelectField(f => f.Rider3Name)
                                            .SelectField(f => f.Rider3Status)
                                            .SelectField(f => f.Rider4Name)
                                            .SelectField(f => f.Rider4Status)
                                        );

                OperationResult<IReadOnlyList<CarPool.Web.Library.Driver>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var results = new List<Driver>();
                foreach (var entry in queryResult.Result)
                {
                    results.Add(CDSDriverToAppDriver(entry));
                }
                return results;
            }
        }

        // GET: api/Driver/5
        public async Task<Driver> Get(string id)
        {
            using (Client client = await CdsHelper.CreateClientAsync(this.Request))
            {
                DataRangeSkipClauseBuilder<CarPool.Web.Library.Driver> query = client.GetRelationalEntitySet<CarPool.Web.Library.Driver>()
               .CreateQueryBuilder()
               .Where(pc => pc.PrimaryId == id)
               .Project(pc => pc
                                .SelectField(f => f.PrimaryId)
                                .SelectField(f => f.RouteTitle)
                                .SelectField(f => f.AverageMiles)
                                .SelectField(f => f.RatePerMile)
                                .SelectField(f => f.Name)
                                .SelectField(f => f.DisplayName)
                                .SelectField(f => f.Schedule)
                                .SelectField(f => f.HomeLatitude)
                                .SelectField(f => f.HomeLongitude)
                                .SelectField(f => f.Arrival)
                                .SelectField(f => f.Departure)
                                .SelectField(f => f.Rider1Name)
                                .SelectField(f => f.Rider1Status)
                                .SelectField(f => f.Rider2Name)
                                .SelectField(f => f.Rider2Status)
                                .SelectField(f => f.Rider3Name)
                                .SelectField(f => f.Rider3Status)
                                .SelectField(f => f.Rider4Name)
                                .SelectField(f => f.Rider4Status)
                            );

                OperationResult<IReadOnlyList<CarPool.Web.Library.Driver>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var result = CDSDriverToAppDriver(queryResult.Result.First());
                return result;
            }
        }

        // POST: api/Driver
        public async Task Post([FromBody]Driver value)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var item = AppDriverToCDSDriver(value);

                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Insert(item)
                    .ExecuteAsync();
            }
        }

        // PUT: api/Driver/5
        public async Task Put(string id, [FromBody]Driver value)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var query = client.GetRelationalEntitySet<CarPool.Web.Library.Driver>()
                           .CreateQueryBuilder()
                           .Where(pc => pc.PrimaryId == id)
                           .Project(pc => pc
                                            .SelectField(f => f.PrimaryId)
                                            .SelectField(f => f.RouteTitle)
                                            .SelectField(f => f.AverageMiles)
                                            .SelectField(f => f.RatePerMile)
                                            .SelectField(f => f.Name)
                                            .SelectField(f => f.DisplayName)
                                            .SelectField(f => f.Schedule)
                                            .SelectField(f => f.HomeLatitude)
                                            .SelectField(f => f.HomeLongitude)
                                            .SelectField(f => f.Arrival)
                                            .SelectField(f => f.Departure)
                                            .SelectField(f => f.Rider1Name)
                                            .SelectField(f => f.Rider1Status)
                                            .SelectField(f => f.Rider2Name)
                                            .SelectField(f => f.Rider2Status)
                                            .SelectField(f => f.Rider3Name)
                                            .SelectField(f => f.Rider3Status)
                                            .SelectField(f => f.Rider4Name)
                                            .SelectField(f => f.Rider4Status)
                                    );

                OperationResult<IReadOnlyList<CarPool.Web.Library.Driver>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var updateExecutor = client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional);
                var cdsItem = queryResult.Result.First();
                var updateItem = client.CreateRelationalFieldUpdates<CarPool.Web.Library.Driver>();

                updateItem.Update(pc => pc.Name, value.Name);
                updateItem.Update(pc => pc.Arrival, new DateTimeOffset(value.Arrival.ToUniversalTime()));
                updateItem.Update(pc => pc.Departure, new DateTimeOffset(value.Departure.ToUniversalTime()));
                updateItem.Update(pc => pc.AverageMiles, Convert.ToDecimal(value.AverageMiles));
                updateItem.Update(pc => pc.DisplayName, value.DisplayName);
                updateItem.Update(pc => pc.HomeLatitude, value.Latitude.HasValue ? Convert.ToDecimal(value.Latitude) : 0);
                updateItem.Update(pc => pc.HomeLongitude, value.Longitude.HasValue ? Convert.ToDecimal(value.Longitude) : 0);

                //Handle RatePerMile currency
                Microsoft.CommonDataService.Currency currency = new Microsoft.CommonDataService.Currency();
                currency.Amount = !String.IsNullOrEmpty(value.RatePerMile) ? Convert.ToDecimal(value.RatePerMile) : 0;
                updateItem.Update(pc => pc.RatePerMile, currency);

                updateItem.Update(pc => pc.Rider1Name, value.Rider1);
                updateItem.Update(pc => pc.Rider1Status, value.Rider1Status);
                updateItem.Update(pc => pc.Rider2Name, value.Rider2);
                updateItem.Update(pc => pc.Rider2Status, value.Rider2Status);
                updateItem.Update(pc => pc.Rider3Name, value.Rider3);
                updateItem.Update(pc => pc.Rider3Status, value.Rider3Status);
                updateItem.Update(pc => pc.Rider4Name, value.Rider4);
                updateItem.Update(pc => pc.Rider4Status, value.Rider4Status);
                updateItem.Update(pc => pc.Schedule, value.Schedule);
                
                updateExecutor.Update(cdsItem, updateItem);
                await updateExecutor.ExecuteAsync();
            }
        }

        // DELETE: api/Driver/5
        public async Task Delete(string id)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var query = client.GetRelationalEntitySet<CarPool.Web.Library.Driver>()
                           .CreateQueryBuilder()
                           .Where(pc => pc.PrimaryId == id)
                           .Project(pc => pc.SelectField(f => f.PrimaryId).SelectField(f => f.Name));

                OperationResult<IReadOnlyList<CarPool.Web.Library.Driver>> queryResult = null;
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

        private Driver CDSDriverToAppDriver(CarPool.Web.Library.Driver cdsDriver)
        {
            Driver driver = new Driver();
            driver.Id = cdsDriver.PrimaryId;
            driver.Name = cdsDriver.Name;
            driver.Arrival = cdsDriver.Arrival.HasValue ? cdsDriver.Arrival.Value.ToDateTime() : new DateTime();
            driver.Departure = cdsDriver.Departure.HasValue ? cdsDriver.Departure.Value.ToDateTime() : new DateTime();
            driver.AverageMiles = cdsDriver.AverageMiles.HasValue ? Convert.ToDouble(cdsDriver.AverageMiles) : 0;
            driver.DisplayName = cdsDriver.DisplayName;
            driver.Latitude = cdsDriver.HomeLatitude.HasValue ? Convert.ToDouble(cdsDriver.HomeLatitude) : 0;
            driver.Longitude = cdsDriver.HomeLongitude.HasValue ? Convert.ToDouble(cdsDriver.HomeLongitude) : 0;
            driver.RatePerMile = cdsDriver.RatePerMile.ToString();
            driver.Rider1 = cdsDriver.Rider1Name;
            driver.Rider1Status = cdsDriver.Rider1Status.HasValue ? cdsDriver.Rider1Status.Value : false;
            driver.Rider2 = cdsDriver.Rider2Name;
            driver.Rider2Status = cdsDriver.Rider2Status.HasValue ? cdsDriver.Rider2Status.Value : false;
            driver.Rider3 = cdsDriver.Rider3Name;
            driver.Rider3Status = cdsDriver.Rider3Status.HasValue ? cdsDriver.Rider3Status.Value : false;
            driver.Rider4 = cdsDriver.Rider4Name;
            driver.Rider4Status = cdsDriver.Rider4Status.HasValue ? cdsDriver.Rider4Status.Value : false;
            driver.Schedule = cdsDriver.Schedule;

            return driver;
        }

        private CarPool.Web.Library.Driver AppDriverToCDSDriver(Driver driver)
        {
            CarPool.Web.Library.Driver cdsDriver = new CarPool.Web.Library.Driver();

            cdsDriver.PrimaryId = driver.Id;
            UpdateCDSDriverFromAppDriver(cdsDriver, driver);

            return cdsDriver;
        }

        private void UpdateCDSDriverFromAppDriver(CarPool.Web.Library.Driver cdsDriver, Driver driver)
        {
            //Could check for id match before update
            //cdsEmployee.PrimaryId = employee.Id;

            cdsDriver.Name = driver.Name;
            cdsDriver.Arrival = new DateTimeOffset(driver.Arrival.ToUniversalTime());
            cdsDriver.Departure = new DateTimeOffset(driver.Departure.ToUniversalTime());
            cdsDriver.AverageMiles = Convert.ToDecimal(driver.AverageMiles);
            cdsDriver.DisplayName = driver.DisplayName;
            cdsDriver.HomeLatitude = driver.Latitude.HasValue ? Convert.ToDecimal(driver.Latitude) : 0;
            cdsDriver.HomeLongitude = driver.Longitude.HasValue ? Convert.ToDecimal(driver.Longitude) : 0;
            if (cdsDriver.RatePerMile == null)
            {
                cdsDriver.RatePerMile = new Microsoft.CommonDataService.Currency();
            }
            cdsDriver.RatePerMile.Amount = !String.IsNullOrEmpty(driver.RatePerMile) ? Convert.ToDecimal(driver.RatePerMile) : 0;
            cdsDriver.Rider1Name = driver.Rider1;
            cdsDriver.Rider1Status = driver.Rider1Status;
            cdsDriver.Rider2Name = driver.Rider2;
            cdsDriver.Rider2Status = driver.Rider2Status;
            cdsDriver.Rider3Name = driver.Rider3;
            cdsDriver.Rider3Status = driver.Rider3Status;
            cdsDriver.Rider4Name = driver.Rider4;
            cdsDriver.Rider4Status = driver.Rider4Status;
            cdsDriver.Schedule = driver.Schedule;
        }
    }
}
