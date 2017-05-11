using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Graph.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Microsoft.Graph;
using CarPool.Clients.Core.Services.Data;

namespace CarPool.Clients.Core.Helpers
{
    public class UserHelper
    {
        private readonly IUserService _userService;

        private readonly IDataService _dataService;

        public UserHelper(
            IUserService userService,
            IDataService dataService)
        {
            _userService = userService;
            _dataService = dataService;
        }

        public async static Task<bool> GetUserWorkLocation(GraphUser user)
        {
            if (!string.IsNullOrEmpty(user.WorkAddress))
            {
                try
                {
                    IEnumerable<Position> geoposition = await MapHelper.GeoCodeAsync(user.WorkAddress);

                    if (geoposition != null && geoposition.Any())
                    {
                        user.WorkLatitude = geoposition.First().Latitude;
                        user.WorkLongitude = geoposition.First().Longitude;
                        return true;
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("Error in geocoder service.");
                }
            }

            return false;
        }

        public async static Task<bool> GetUserHomeLocation(GraphUser user)
        {
            if (!string.IsNullOrEmpty(user.HomeAddress))
            {
                try
                {
                    IEnumerable<Position> geoposition = await MapHelper.GeoCodeAsync(user.HomeAddress);

                    if (geoposition != null && geoposition.Any())
                    {
                        user.Latitude = geoposition.First().Latitude;
                        user.Longitude = geoposition.First().Longitude;
                        return true;
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("Error in geocoder service.");
                }
            }

            return false;
        }

        public static double CalculateDistance(GraphUser me, GraphUser user)
        {
            if (me.HasLocation() && user.HasLocation())
            {
                var dist = MapHelper.CalculateDistance(
                    me.Location().Value.Latitude,
                    me.Location().Value.Longitude,
                    user.Location().Value.Latitude,
                    user.Location().Value.Longitude,
                    DistanceMeasure.Miles);

                return dist;
            }

            return Double.NaN;
        }

        public static double GetTripTimeEstimationAsync(Position? me, Position? destination)
        {
            try
            {
                if (me.HasValue && destination.HasValue) {

                    var distance = MapHelper.CalculateDistance(
                        me.Value.Latitude,
                        me.Value.Longitude,
                        destination.Value.Latitude,
                        destination.Value.Longitude,
                        DistanceMeasure.Miles);

                    if (distance != Double.NaN)
                    {
                        return (distance / AppSettings.DefaultMphSpeed) * 60;
                    }
                }
                
                return AppSettings.DefaultTripTime;
            }
            catch
            {
                return AppSettings.DefaultTripTime;
            }
        }

        public async Task<bool> UpdateProfilesChanges(GraphServiceUsersCollectionResponse deltaChanges, Employee employee)
        {
            if (deltaChanges?.Value != null)
            {
                // Update delta token
                if (deltaChanges.AdditionalData != null && 
                    deltaChanges.AdditionalData.ContainsKey("@odata.deltaLink"))
                {
                    Settings.DeltaLink = (string)deltaChanges.AdditionalData["@odata.deltaLink"];
                }
                else
                {
                    // TODO pending nextLink iterations
                }

                // update employee profile
                var deltaEmployee = deltaChanges.Value.FirstOrDefault(e => employee.Email.Equals(e.Mail));
                
                if (deltaEmployee != null)
                {
                    employee.Name = deltaEmployee.DisplayName;
                    employee.Email = deltaEmployee.Mail;
                    employee.PhoneNo = deltaEmployee.BusinessPhones?.FirstOrDefault();
                    if (!employee.HomeAddress.Equals($"{deltaEmployee.StreetAddress}, {deltaEmployee.City} {deltaEmployee.PostalCode}"))
                    {
                        employee.HomeAddress = $"{deltaEmployee.StreetAddress}, {deltaEmployee.City} {deltaEmployee.PostalCode}";
                        var user = new GraphUser(employee);
                        if (await GetUserHomeLocation(user)) {
                            employee.Longitude = user.Longitude.Value;
                            employee.Latitude = user.Latitude.Value;
                        }
                    }

                    await _dataService.InsertOrUpdateEmployeeAsync(employee);
                    return true;
                }
            }

            return false;
        }
    }
}