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
    /// Controller for RideDetails
    /// This controller uses the default routing table constructed using method signature
    /// Learn more here: https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    /// </summary>
    public class RideDetailsController : ApiController
    {
        // GET: api/RideDetails
        public async Task<IEnumerable<RideDetails>> Get()
        {
            using (Client client = await CdsHelper.CreateClientAsync(this.Request))
            {
                DataRangeSkipClauseBuilder<CarPool.Web.Library.RideShare> query = client.GetRelationalEntitySet<CarPool.Web.Library.RideShare>()
                           .CreateQueryBuilder()
                           .Project(pc => pc
                                            .SelectField(f => f.PrimaryId)
                                            .SelectField(f => f.ExpenseReportSubmitted)
                                            .SelectField(f => f.ExpenseReportApproved)
                                            .SelectField(f => f.Passenger1OnRide)
                                            .SelectField(f => f.Passenger2OnRide)
                                            .SelectField(f => f.Passenger3OnRide)
                                            .SelectField(f => f.Passenger4OnRide)
                                            .SelectField(f => f.RideCompleted)
                                            .SelectField(f => f.FinanceApprovalStatus)
                                            .SelectField(f => f.Passenger1Name)
                                            .SelectField(f => f.Passenger2Name)
                                            .SelectField(f => f.Passenger3Name)
                                            .SelectField(f => f.Passenger4Name)
                                            .SelectField(f => f.DistanceString)
                                            .SelectField(f => f.RideDateTime)
                                            .SelectField(f => f.DriverName)
                                            .SelectField(f => f.RouteDescription)
                                            .SelectField(f => f.DistanceString)
                                            .SelectField(f => f.Distance)
                                        );

                OperationResult<IReadOnlyList<CarPool.Web.Library.RideShare>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var results = new List<RideDetails>();
                foreach (var entry in queryResult.Result)
                {
                    results.Add(CDSRideDetailsToAppRideDetails(entry));
                }
                return results;
            }
        }

        // GET: api/RideDetails/5
        public async Task<RideDetails> Get(string id)
        {
            using (Client client = await CdsHelper.CreateClientAsync(this.Request))
            {
                DataRangeSkipClauseBuilder<CarPool.Web.Library.RideShare> query = client.GetRelationalEntitySet<CarPool.Web.Library.RideShare>()
               .CreateQueryBuilder()
               .Where(pc => pc.PrimaryId == id)
               .Project(pc => pc
                                .SelectField(f => f.PrimaryId)
                                .SelectField(f => f.ExpenseReportSubmitted)
                                .SelectField(f => f.ExpenseReportApproved)
                                .SelectField(f => f.Passenger1OnRide)
                                .SelectField(f => f.Passenger2OnRide)
                                .SelectField(f => f.Passenger3OnRide)
                                .SelectField(f => f.Passenger4OnRide)
                                .SelectField(f => f.RideCompleted)
                                .SelectField(f => f.FinanceApprovalStatus)
                                .SelectField(f => f.Passenger1Name)
                                .SelectField(f => f.Passenger2Name)
                                .SelectField(f => f.Passenger3Name)
                                .SelectField(f => f.Passenger4Name)
                                .SelectField(f => f.DistanceString)
                                .SelectField(f => f.RideDateTime)
                                .SelectField(f => f.DriverName)
                                .SelectField(f => f.RouteDescription)
                                .SelectField(f => f.DistanceString)
                                .SelectField(f => f.Distance)
                            );

                OperationResult<IReadOnlyList<CarPool.Web.Library.RideShare>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var result = CDSRideDetailsToAppRideDetails(queryResult.Result.First());
                return result;
            }
        }

        // POST: api/RideDetails
        public async Task Post([FromBody]RideDetails value)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var item = AppRideDetailsToCDSRideDetails(value);

                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Insert(item)
                    .ExecuteAsync();
            }
        }

        // PUT: api/RideDetails/5
        public async Task Put(string id, [FromBody]RideDetails value)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var query = client.GetRelationalEntitySet<CarPool.Web.Library.RideShare>()
                           .CreateQueryBuilder()
                           .Where(pc => pc.PrimaryId == id)
                           .Project(pc => pc
                                            .SelectField(f => f.PrimaryId)
                                            .SelectField(f => f.ExpenseReportSubmitted)
                                            .SelectField(f => f.ExpenseReportApproved)
                                            .SelectField(f => f.Passenger1OnRide)
                                            .SelectField(f => f.Passenger2OnRide)
                                            .SelectField(f => f.Passenger3OnRide)
                                            .SelectField(f => f.Passenger4OnRide)
                                            .SelectField(f => f.RideCompleted)
                                            .SelectField(f => f.FinanceApprovalStatus)
                                            .SelectField(f => f.Passenger1Name)
                                            .SelectField(f => f.Passenger2Name)
                                            .SelectField(f => f.Passenger3Name)
                                            .SelectField(f => f.Passenger4Name)
                                            .SelectField(f => f.DistanceString)
                                            .SelectField(f => f.RideDateTime)
                                            .SelectField(f => f.DriverName)
                                            .SelectField(f => f.RouteDescription)
                                            .SelectField(f => f.DistanceString)
                                            .SelectField(f => f.Distance)
                                    );

                OperationResult<IReadOnlyList<CarPool.Web.Library.RideShare>> queryResult = null;
                await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                    .Query(query, out queryResult)
                    .ExecuteAsync();

                var updateExecutor = client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional);
                var cdsItem = queryResult.Result.First();
                var updateItem = client.CreateRelationalFieldUpdates<CarPool.Web.Library.RideShare>();

                updateItem.Update(pc => pc.DriverName, value.Driver);
                updateItem.Update(pc => pc.Distance, Convert.ToDecimal(value.Distance));
                updateItem.Update(pc => pc.ExpenseReportSubmitted, value.EmployeeSubmissionStatus);
                updateItem.Update(pc => pc.RideDateTime, new DateTimeOffset(value.Date.ToUniversalTime()));
                updateItem.Update(pc => pc.FinanceApprovalStatus, value.FinanceApprovalStatus);
                updateItem.Update(pc => pc.ExpenseReportApproved, value.ManagerApprovalStatus);
                updateItem.Update(pc => pc.RouteDescription, value.Route);
                updateItem.Update(pc => pc.Passenger1Name, value.Rider1);
                updateItem.Update(pc => pc.Passenger1OnRide, value.Rider1Status);
                updateItem.Update(pc => pc.Passenger2Name, value.Rider2);
                updateItem.Update(pc => pc.Passenger2OnRide, value.Rider2Status);
                updateItem.Update(pc => pc.Passenger3Name, value.Rider3);
                updateItem.Update(pc => pc.Passenger3OnRide, value.Rider3Status);
                updateItem.Update(pc => pc.Passenger4Name, value.Rider4);
                updateItem.Update(pc => pc.Passenger4OnRide, value.Rider4Status);
                
                updateExecutor.Update(cdsItem, updateItem);
                await updateExecutor.ExecuteAsync();
            }
        }

        // DELETE: api/RideDetails/5
        public async Task Delete(string id)
        {
            using (var client = await CdsHelper.CreateClientAsync(this.Request))
            {
                var query = client.GetRelationalEntitySet<CarPool.Web.Library.RideShare>()
                           .CreateQueryBuilder()
                           .Where(pc => pc.PrimaryId == id)
                           .Project(pc => pc.SelectField(f => f.PrimaryId).SelectField(f => f.DriverName));

                OperationResult<IReadOnlyList<CarPool.Web.Library.RideShare>> queryResult = null;
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

        private RideDetails CDSRideDetailsToAppRideDetails(CarPool.Web.Library.RideShare cdsRideDetails)
        {
            RideDetails rideDetails = new RideDetails();
            rideDetails.Id = cdsRideDetails.PrimaryId;
            rideDetails.Driver = cdsRideDetails.DriverName;
            rideDetails.Distance = cdsRideDetails.Distance.HasValue ? Convert.ToDouble(cdsRideDetails.Distance) : 0;
            rideDetails.EmployeeSubmissionStatus = cdsRideDetails.ExpenseReportSubmitted.HasValue ? cdsRideDetails.ExpenseReportSubmitted.Value : false;
            rideDetails.Date = cdsRideDetails.RideDateTime.HasValue ? cdsRideDetails.RideDateTime.Value.ToDateTime() : new DateTime();
            rideDetails.FinanceApprovalStatus = cdsRideDetails.FinanceApprovalStatus.HasValue ? cdsRideDetails.FinanceApprovalStatus.Value : false;
            rideDetails.ManagerApprovalStatus = cdsRideDetails.ExpenseReportApproved.HasValue ? cdsRideDetails.ExpenseReportApproved.Value : false;
            rideDetails.Route = cdsRideDetails.RouteDescription;
            rideDetails.Rider1 = cdsRideDetails.Passenger1Name;
            rideDetails.Rider1Status = cdsRideDetails.Passenger1OnRide.HasValue ? cdsRideDetails.Passenger1OnRide.Value : false;
            rideDetails.Rider2 = cdsRideDetails.Passenger2Name;
            rideDetails.Rider2Status = cdsRideDetails.Passenger2OnRide.HasValue ? cdsRideDetails.Passenger2OnRide.Value : false;
            rideDetails.Rider3 = cdsRideDetails.Passenger3Name;
            rideDetails.Rider3Status = cdsRideDetails.Passenger3OnRide.HasValue ? cdsRideDetails.Passenger3OnRide.Value : false;
            rideDetails.Rider4 = cdsRideDetails.Passenger4Name;
            rideDetails.Rider4Status = cdsRideDetails.Passenger4OnRide.HasValue ? cdsRideDetails.Passenger4OnRide.Value : false;

            return rideDetails;
        }

        private CarPool.Web.Library.RideShare AppRideDetailsToCDSRideDetails(RideDetails rideDetails)
        {
            CarPool.Web.Library.RideShare cdsRideDetails = new CarPool.Web.Library.RideShare();

            cdsRideDetails.PrimaryId = rideDetails.Id;
            UpdateCDSRideDetailsFromAppRideDetails(cdsRideDetails, rideDetails);

            return cdsRideDetails;
        }

        private void UpdateCDSRideDetailsFromAppRideDetails(CarPool.Web.Library.RideShare cdsRideDetails, RideDetails rideDetails)
        {
            //Could check for id match before update
            //cdsRideDetails.PrimaryId = rideDetails.Id;

            cdsRideDetails.DriverName = rideDetails.Driver;
            cdsRideDetails.Distance = Convert.ToDecimal(rideDetails.Distance);
            cdsRideDetails.ExpenseReportSubmitted = rideDetails.EmployeeSubmissionStatus;
            cdsRideDetails.RideDateTime = new DateTimeOffset(rideDetails.Date.ToUniversalTime());
            cdsRideDetails.FinanceApprovalStatus = rideDetails.FinanceApprovalStatus;
            cdsRideDetails.ExpenseReportApproved = rideDetails.ManagerApprovalStatus;
            cdsRideDetails.RouteDescription = rideDetails.Route;
            cdsRideDetails.Passenger1Name = rideDetails.Rider1;
            cdsRideDetails.Passenger1OnRide = rideDetails.Rider1Status;
            cdsRideDetails.Passenger2Name = rideDetails.Rider2;
            cdsRideDetails.Passenger2OnRide = rideDetails.Rider2Status;
            cdsRideDetails.Passenger3Name = rideDetails.Rider3;
            cdsRideDetails.Passenger3OnRide = rideDetails.Rider3Status;
            cdsRideDetails.Passenger4Name = rideDetails.Rider4;
            cdsRideDetails.Passenger4OnRide = rideDetails.Rider4Status;

            return;
        }
    }    
}
