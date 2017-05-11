using Microsoft.CommonDataService;
using Microsoft.CommonDataService.CommonEntitySets;
using Microsoft.CommonDataService.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPool.Web.Library
{
    [RelationalEntitySet("Microsoft.CommonDataService.CommonEntitySets", "FabrikamRideShare_", Version = "1.0")]
    [RelationalIndex("Idx1", true, new[] { "PrimaryId" }, Namespace = "Microsoft.CommonDataService.CommonEntitySets")]
    public class RideShare : TypedRelationalEntity
    {
        public RideShare() { }

        [RelationalField("PrimaryId", IsMandatory = false)]
        [AutoNumber]
        public string PrimaryId { get; set; }
        
        [RelationalField("ExpenseReportSubmitted", IsMandatory = false)]
        public bool? ExpenseReportSubmitted { get; set; }

        [RelationalField("ExpenseReportApproved", IsMandatory = false)]
        public bool? ExpenseReportApproved { get; set; }

        [RelationalField("Passenger1OnRide", IsMandatory = false)]
        public bool? Passenger1OnRide { get; set; }

        [RelationalField("Passenger2OnRide", IsMandatory = false)]
        public bool? Passenger2OnRide { get; set; }

        [RelationalField("Passenger3OnRide", IsMandatory = false)]
        public bool? Passenger3OnRide { get; set; }

        [RelationalField("Passenger4OnRide", IsMandatory = false)]
        public bool? Passenger4OnRide { get; set; }

        [RelationalField("RideCompleted", IsMandatory = false)]
        public bool? RideCompleted { get; set; }

        [RelationalField("FinanceApprovalStatus", IsMandatory = false)]
        public bool? FinanceApprovalStatus { get; set; }

        [RelationalField("Passenger1Name", IsMandatory = false)]
        public string Passenger1Name { get; set; }

        [RelationalField("Passenger2Name", IsMandatory = false)]
        public string Passenger2Name { get; set; }

        [RelationalField("Passenger3Name", IsMandatory = false)]
        public string Passenger3Name { get; set; }

        [RelationalField("Passenger4Name", IsMandatory = false)]
        public string Passenger4Name { get; set; }

        [RelationalField("DistanceString", IsMandatory = false)]
        public string DistanceString { get; set; }

        [RelationalField("RideDateTime", IsMandatory = false)]
        public UtcDateTime? RideDateTime { get; set; }

        [RelationalField("DriverName", IsMandatory = false)]
        public string DriverName { get; set; }

        [RelationalField("RouteDescription", IsMandatory = false)]
        public string RouteDescription { get; set; }

        [RelationalField("Distance", IsMandatory = false)]
        public decimal? Distance { get; set; }
    }
}
