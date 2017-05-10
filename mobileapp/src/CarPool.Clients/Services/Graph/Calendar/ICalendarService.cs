using Microsoft.Graph;
using System;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Calendar
{
    public interface ICalendarService
    {
        Task<IUserCalendarViewCollectionPage> GetDayEventsAsync(DateTime startDateTime, DateTime endDateTime);

        Task<IUserCalendarViewCollectionPage> GetCarpoolEventsAsync(DateTime startDateTime, DateTime endDateTime);

        Task<string> CreateEventAsync(DateTime startDateTime, DateTime endDateTime, string eventDescription, string eventLocation, string eventSubject, string[] eventAttendees, bool isAllDay, string aditionalProperty, PatternedRecurrence pattern);
        
        Task DeleteEventAsync(string eventId);

        Task<bool> UpdateEventAsync(string eventId, string eventAttendees, string eventDescription, string eventLocation, string eventSubject, DateTime startDateTime, DateTime endDateTime, bool isAllDayMeeting);

        Task<bool> CancelEventAsync(string eventId);
    }
}