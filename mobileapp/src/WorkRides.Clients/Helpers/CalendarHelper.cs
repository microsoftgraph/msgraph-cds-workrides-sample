using Microsoft.Graph;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace CarPool.Clients.Core.Helpers
{
    public class CalendarHelper
    {

        // Returns work range based on the calendars events passed as parameter.
        public static void CalculateWeekdaysCalendarWorkTime(IUserCalendarViewCollectionPage events, out DateTime arrivalTime, out DateTime departureTime)
        {
            TimeSpan arrivalStartDateTime = AppSettings.DefaultArrivalTime;
            TimeSpan arrivalEndDateTime = AppSettings.DefaultArrivalTime.Add(new TimeSpan(2, 0, 0));

            TimeSpan departureStartDateTime = AppSettings.DefaultDepartureTime;
            TimeSpan departureEndDateTime = AppSettings.DefaultArrivalTime.Add(new TimeSpan(2, 0, 0));

            arrivalTime = DateTime.Today + AppSettings.DefaultArrivalTime;
            departureTime = DateTime.Today + AppSettings.DefaultDepartureTime;

            try
            {
                // Get first and last time by day
                var eventsByDay = events?
                    .Where(e => !string.IsNullOrEmpty(e?.Start?.DateTime))
                    .Where(e => !e.IsCancelled.HasValue || !e.IsCancelled.Value)
                    .GroupBy(e => ParseLocalDate(e.Start.DateTime).ToString("MM/dd/yyyy"), (key, g) => new
                    {
                        StartDate = key,
                        First = ParseLocalDate(g.First().Start.DateTime),
                        Last = ParseLocalDate(g.Last().End.DateTime)
                    });

                if (eventsByDay != null && eventsByDay.Any())
                {
                    arrivalTime = eventsByDay.OrderBy(d => d.First.TimeOfDay).FirstOrDefault().First;
                    departureTime = eventsByDay.OrderBy(d => d.Last.TimeOfDay).LastOrDefault().Last;

                    // Range validations
                    if (arrivalTime.TimeOfDay < arrivalStartDateTime ||
                        arrivalTime.TimeOfDay > arrivalEndDateTime)
                    {
                        arrivalTime = DateTime.Today + AppSettings.DefaultArrivalTime;
                    }

                    if (departureTime.TimeOfDay < departureStartDateTime ||
                        departureTime.TimeOfDay > departureEndDateTime)
                    {
                        departureTime = DateTime.Today + AppSettings.DefaultDepartureTime;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private static DateTime ParseLocalDate(string date)
        {
            DateTime startDate;
            DateTime.TryParse(
                date,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out startDate);
            return startDate.ToLocalTime();
        }
    }
}
