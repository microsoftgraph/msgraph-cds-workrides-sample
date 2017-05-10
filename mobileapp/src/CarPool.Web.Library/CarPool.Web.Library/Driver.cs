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
    [RelationalEntitySet("Microsoft.CommonDataService.CommonEntitySets", "FabrikamDrivers_", Version = "1.0")]
    [RelationalIndex("Idx1", true, new[] { "PrimaryId" }, Namespace = "Microsoft.CommonDataService.CommonEntitySets")]
    public class Driver : TypedRelationalEntity
    {
        public Driver() { }

        [RelationalField("PrimaryId", IsMandatory = false)]
        [AutoNumber]
        public string PrimaryId { get; set; }

        [RelationalField("RouteTitle", IsMandatory = false)]
        public string RouteTitle { get; set; }

        [RelationalField("AverageMiles", IsMandatory = false)]
        public decimal? AverageMiles { get; set; }

        [RelationalField("RatePerMile", IsMandatory = false)]
        public Currency RatePerMile { get; set; }

        [RelationalField("Name", IsMandatory = false)]
        public string Name { get; set; }

        [RelationalField("DisplayName", IsMandatory = false)]
        public string DisplayName { get; set; }

        [RelationalField("Schedule", IsMandatory = false)]
        public string Schedule { get; set; }

        [RelationalField("HomeLatitude", IsMandatory = false)]
        public decimal? HomeLatitude { get; set; }

        [RelationalField("HomeLongitude", IsMandatory = false)]
        public decimal? HomeLongitude { get; set; }

        [RelationalField("Arrival", IsMandatory = false)]
        public UtcDateTime? Arrival { get; set; }

        [RelationalField("Departure", IsMandatory = false)]
        public UtcDateTime? Departure { get; set; }

        [RelationalField("Rider1Name", IsMandatory = false)]
        public string Rider1Name { get; set; }

        [RelationalField("Rider1Status", IsMandatory = false)]
        public bool? Rider1Status { get; set; }

        [RelationalField("Rider2Name", IsMandatory = false)]
        public string Rider2Name { get; set; }

        [RelationalField("Rider2Status", IsMandatory = false)]
        public bool? Rider2Status { get; set; }

        [RelationalField("Rider3Name", IsMandatory = false)]
        public string Rider3Name { get; set; }

        [RelationalField("Rider3Status", IsMandatory = false)]
        public bool? Rider3Status { get; set; }

        [RelationalField("Rider4Name", IsMandatory = false)]
        public string Rider4Name { get; set; }

        [RelationalField("Rider4Status", IsMandatory = false)]
        public bool? Rider4Status { get; set; }
    }
}
