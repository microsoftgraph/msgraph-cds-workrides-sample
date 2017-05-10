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
    [RelationalEntitySet("Microsoft.CommonDataService.CommonEntitySets", "FabrikamEmployee_", Version = "1.0")]
    [RelationalIndex("Idx1", true, new[] { "PrimaryId" }, Namespace = "Microsoft.CommonDataService.CommonEntitySets")]
    public class Employee : TypedRelationalEntity
    {
        public Employee() { }

        [RelationalField("PrimaryId", IsMandatory = true)]
        [AutoNumber]
        public string PrimaryId { get; set; }

        [RelationalField("FullName", IsMandatory = true)]
        public string FullName { get; set; }

        [RelationalField("HomeAddress", IsMandatory = false)]
        public Address HomeAddress { get; set; }

        [RelationalField("WorkAddress", IsMandatory = false)]
        public Address WorkAddress { get; set; }

        [RelationalField("PreferredArrivalTimeAtWork", IsMandatory = false)]
        public UtcDateTime? PreferredArrivalTimeAtWork { get; set; }

        [RelationalField("PreferredDepartureTimeFromWork", IsMandatory = false)]
        public UtcDateTime? PreferredDepartureTimeFromWork { get; set; }

        [RelationalField("Email", IsMandatory = false)]
        public string Email { get; set; }

        [RelationalField("Phone", IsMandatory = false)]
        public string Phone { get; set; }

        [RelationalField("BusinessUnit", IsMandatory = false)]
        public string BusinessUnit { get; set; }

        [RelationalField("HomeLatitude", IsMandatory = false)]
        public decimal? HomeLatitude { get; set; }

        [RelationalField("HomeLongitude", IsMandatory = false)]
        public decimal? HomeLongitude { get; set; }

        [RelationalField("WorkLatitude", IsMandatory = false)]
        public decimal? WorkLatitude { get; set; }

        [RelationalField("WorkLongitude", IsMandatory = false)]
        public decimal? WorkLongitude { get; set; }
    }
}
