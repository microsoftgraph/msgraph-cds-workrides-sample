using System;

namespace CarPool.Clients.Core.Models
{
    /// <summary>
    /// This is essentially the rides happening every day and for each day there will be one or more records 
    /// depending up on how many drivers exist. For example if there are 10 drivers there will be 10 rides for 
    /// that particular day. This table is populated along with when the calendar event is setup. 
    /// To keep it simple this table will be populated for Jan – Jun. 
    /// Some of these (especially the Status fields are used by PowerApp)
    /// </summary>
    public class RideDetails
    {
        public string Id { get; set; }
        public string Driver { get; set; }
        public DateTime Date { get; set; }
        public string Route { get; set; }
        public string Distance { get; set; }
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