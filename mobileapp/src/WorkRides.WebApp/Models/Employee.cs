using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarPool.WebApp.Models
{
    public class Employee
    {
        //[PrimaryKey]
        //[NotNull]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        //[MaxLength(12)]
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