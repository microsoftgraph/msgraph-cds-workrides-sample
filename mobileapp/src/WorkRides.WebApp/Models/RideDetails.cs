using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarPool.WebApp.Models
{
    public class RideDetails
    {
        public string Id { get; set; }
        public string Driver { get; set; }
        public DateTime Date { get; set; }
        public string Route { get; set; }
        public double Distance { get; set; }
        public string Rider1 { get; set; }
        public string Rider2 { get; set; }
        public string Rider3 { get; set; }
        public string Rider4 { get; set; }
        public bool Rider1Status { get; set; }
        public bool Rider2Status { get; set; }
        public bool Rider3Status { get; set; }
        public bool Rider4Status { get; set; }
        // Denotes if employee has submitted the expense or not
        public bool EmployeeSubmissionStatus { get; set; }
        // Denotes if manager has approved or not
        public bool ManagerApprovalStatus { get; set; }
        // Denotes if HR has done final approval
        public bool FinanceApprovalStatus { get; set; }
    }

}