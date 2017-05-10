using System;

namespace CarPool.Clients.Core.Models
{
    /// <summary>
    /// This would contain all the employees who have signed in to the app. 
    /// This will contain the updated user profile. 
    /// The initial user profile is coming from MS Graph during the first sign in to the app. 
    /// The user profile can be updated by the app either during the first sign in or from the user profile feature. 
    /// During such updates the app updates the back end table with the updated information.
    /// </summary>
    public class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string HomeAddress { get; set; }
        public string WorkAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double WorkLatitude { get; set; }
        public double WorkLongitude { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
    }
}