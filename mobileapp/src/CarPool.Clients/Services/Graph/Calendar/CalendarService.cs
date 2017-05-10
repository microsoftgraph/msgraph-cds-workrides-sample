using Carpool.Clients.Services.Data;
using CarPool.Clients.Core.Helpers;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Calendar
{
    public class CalendarService : ICalendarService
    {
        private readonly IRequestProvider _requestProvider;

        public CalendarService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<IUserCalendarViewCollectionPage> GetDayEventsAsync(DateTime startDateTime, DateTime endDateTime)
        {
            var options = new List<Option>();

            if (startDateTime != null)
            {
                options.Add(new QueryOption("StartDateTime", startDateTime.ToUniversalTime().ToString("o")));
            }

            if (endDateTime != null)
            {
                options.Add(new QueryOption("EndDateTime", endDateTime.ToUniversalTime().ToString("o")));
            }

            return await GraphClient.Instance.Beta.Me.CalendarView
                .Request(options)
                .Select("id,start,end,isallday,iscancelled")
                .OrderBy("start/DateTime")
                .GetAsync();
        }

        public async Task<IUserCalendarViewCollectionPage> GetCarpoolEventsAsync(DateTime startDateTime, DateTime endDateTime)
        {
            var options = new List<Option>();

            if (startDateTime != null)
            {
                options.Add(new QueryOption("StartDateTime", startDateTime.ToUniversalTime().ToString("o")));
            }

            if (endDateTime != null)
            {
                options.Add(new QueryOption("EndDateTime", endDateTime.ToUniversalTime().ToString("o")));
            }

            return await GraphClient.Instance.Beta.Me.CalendarView.Request(options)
                .Filter($"subject eq '{AppSettings.CarpoolEventSubject}'")
                .Select("id,start,end,isallday,iscancelled,body,bodypreview")
                .OrderBy("start/DateTime")
                .Top(AppSettings.ScheduleTopItems)
                .GetAsync();
        }

        public async Task<string> CreateEventAsync(DateTime startDateTime, DateTime endDateTime, string eventDescription, string eventLocation, string eventSubject, string[] eventAttendees, bool isAllDay, string aditionalProperty, PatternedRecurrence pattern)
        {
            string createdEventId = null;

            // Prepare the List of attendees
            // Prepare the recipient list
            List<Attendee> attendeesList = new List<Attendee>();
            foreach (string attendee in eventAttendees)
            {
                attendeesList.Add(new Attendee {
                    EmailAddress = new EmailAddress { Address = attendee.Trim() },
                    Type = AttendeeType.Required
                });
            }

            // Event body
            var eventBody = new ItemBody();
            eventBody.Content = string.Format("{0} - {1}{2}", 
                eventDescription,
                AppSettings.CarpoolEventBody,
                aditionalProperty);
            eventBody.ContentType = BodyType.Text;

            // Event start and end time
            var eventStartTime = new DateTimeTimeZone();
            eventStartTime.DateTime = startDateTime.ToString("o");
            eventStartTime.TimeZone = TimeZoneInfo.Local.ToString();
            var eventEndTime = new DateTimeTimeZone();
            eventEndTime.TimeZone = TimeZoneInfo.Local.ToString();
            eventEndTime.DateTime = endDateTime.ToString("o");

            // Create an event to add to the events collection
            var location = new Location();
            location.DisplayName = eventLocation;
            var newEvent = new Event();
            newEvent.Subject = eventSubject;
            newEvent.Location = location;
            newEvent.Attendees = attendeesList;
            newEvent.Body = eventBody;
            newEvent.Start = eventStartTime;
            newEvent.End = eventEndTime;

            if (pattern != null)
            {
                newEvent.Recurrence = pattern;
            }

            if (isAllDay)
            {
                newEvent.IsAllDay = true;
            }

            var createdEvent = await GraphClient.Instance.Beta.Me.Events.Request().AddAsync(newEvent);
            createdEventId = createdEvent.Id;

            return createdEventId;
        }

        public Task DeleteEventAsync(string eventId)
        {
            return GraphClient.Instance.Beta.Me.Events[eventId].Request().DeleteAsync();
        }

        /// <summary>
        /// Method used to send a cancellation message and cancel the event. 
        /// This method should only be called by the event organizer.
        /// This api need the scope "Calendars.ReadWrite"
        /// doc: https://developer.microsoft.com/en-us/graph/docs/api-reference/beta/api/event_cancel
        /// </summary>
        /// <param name="eventId">The event to cancel</param>
        /// <returns>true if event has been correctly canceled.</returns>
        public async Task<bool> CancelEventAsync(string eventId)
        {
            if (!string.IsNullOrEmpty(eventId))
            {
                var authToken = Settings.TokenForUser;

                if (string.IsNullOrEmpty(authToken))
                {
                    return false;
                }

                try
                {
                    UriBuilder builder = new UriBuilder(AppSettings.GraphApiEndpoint);
                    builder.Path = $"beta/me/events/{eventId}/cancel";

                    string uri = builder.ToString();

                    return await _requestProvider.PostAsync<string, bool>(uri, "" , authToken);
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> UpdateEventAsync(string eventId, string eventAttendees, string eventDescription, string eventLocation, string eventSubject, DateTime startDateTime, DateTime endDateTime, bool isAllDayMeeting)
        {
            bool eventUpdated = false;

            var eventToUpdate = new Event();

            // Prepare the List of attendees
            // Prepare the recipient list
            string[] splitter = { ";" };
            var splitRecipientsString = eventAttendees.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            List<Attendee> attendeesList = new List<Attendee>();
            foreach (string attendee in splitRecipientsString)
            {
                attendeesList.Add(new Attendee { EmailAddress = new EmailAddress { Address = attendee.Trim() }, Type = AttendeeType.Required });
            }

            // Event body
            var eventBody = new ItemBody();
            eventBody.Content = eventDescription;
            eventBody.ContentType = BodyType.Text;

            var eventStartTime = new DateTimeTimeZone();
            var eventEndTime = new DateTimeTimeZone();

            // Event start and end time
            eventStartTime.DateTime = startDateTime.ToString("o");
            eventStartTime.TimeZone = "UTC";
            eventEndTime.TimeZone = "UTC";
            eventEndTime.DateTime = endDateTime.ToString("o");
            
            // Create an event to add to the events collection
            var location = new Location();
            location.DisplayName = eventLocation;

            eventToUpdate.Subject = eventSubject;
            eventToUpdate.Location = location;
            eventToUpdate.Attendees = attendeesList;
            eventToUpdate.Body = eventBody;
            eventToUpdate.Start = eventStartTime;
            eventToUpdate.End = eventEndTime;

            var updatedEvent = await GraphClient.Instance.Beta.Me.Events[eventId].Request()
                .UpdateAsync(eventToUpdate);

            if (updatedEvent != null)
            {
                eventUpdated = true;
            }

            return eventUpdated;
        }
    }
}