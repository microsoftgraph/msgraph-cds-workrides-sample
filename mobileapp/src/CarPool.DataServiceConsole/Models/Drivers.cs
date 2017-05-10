using Microsoft.WindowsAzure.MobileServices;
using System;

namespace CarPool.Clients.Core.Models
{
    /// <summary>
    ///  This would contain all the drivers and the details of the rides they have created. 
    ///  All the information related to the particular ride (e.g. interested riders and their approval status etc.) 
    ///  can be tracked here. 
    ///  The app will reflect the status. This is created when a user has signed up to drive.
    /// </summary>
    public class Driver
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Schedule { get; set; }
        public double AverageMiles { get; set; }
        public string RatePerMile { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public string Rider1 { get; set; }
        public bool Rider1Status { get; set; }
        public string Rider2 { get; set; }
        public bool Rider2Status { get; set; }
        public string Rider3 { get; set; }
        public bool Rider3Status { get; set; }
        public string Rider4 { get; set; }
        public bool Rider4Status { get; set; }

        [CreatedAt]
        public DateTimeOffset CreatedOn { get; set; }
        [UpdatedAt]
        public DateTimeOffset Updated { get; set; }
        [Version]
        public string AzureVersion { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Driver)
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    return Name.Equals(((Driver)obj).Name);
                }
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name.GetHashCode();
            }

            return base.GetHashCode();
        }
    }
}