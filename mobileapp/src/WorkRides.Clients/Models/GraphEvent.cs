using Microsoft.Graph;
using System;
using System.Globalization;

namespace CarPool.Clients.Core.Models
{
    public class GraphEvent : Event
    {
        public GraphEvent() : base()
        {
            this.IsCancelled = false;
            this.Body = null;
        }

        public GraphEvent(Event e) : base()
        {
            this.AdditionalData = e.AdditionalData;
            this.Attachments = e.Attachments;
            this.Attendees = e.Attendees;
            this.Body = e.Body;
            this.BodyPreview = e.BodyPreview;
            this.Calendar = e.Calendar;
            this.Categories = e.Categories;
            this.ChangeKey = e.ChangeKey;
            this.CreatedDateTime = e.CreatedDateTime;
            this.End = e.End;
            this.Extensions = e.Extensions;
            this.HasAttachments = e.HasAttachments;
            this.ICalUId = e.ICalUId;
            this.Id = e.Id;
            this.Importance = e.Importance;
            this.Instances = e.Instances;
            this.IsAllDay = e.IsAllDay;
            this.IsCancelled = e.IsCancelled;
            this.IsOrganizer = e.IsOrganizer;
            this.IsReminderOn = e.IsReminderOn;
            this.LastModifiedDateTime = e.LastModifiedDateTime;
            this.MultiValueExtendedProperties = e.MultiValueExtendedProperties;
            this.ODataType = e.ODataType;
            this.OnlineMeetingUrl = e.OnlineMeetingUrl;
            this.Organizer = e.Organizer;
            this.OriginalEndTimeZone = e.OriginalEndTimeZone;
            this.OriginalStart = e.OriginalStart;
            this.OriginalStartTimeZone = e.OriginalStartTimeZone;
            this.Recurrence = e.Recurrence;
            this.ReminderMinutesBeforeStart = e.ReminderMinutesBeforeStart;
            this.ResponseRequested = e.ResponseRequested;
            this.ResponseStatus = e.ResponseStatus;
            this.Sensitivity = e.Sensitivity;
            this.SeriesMasterId = e.SeriesMasterId;
            this.ShowAs = e.ShowAs;
            this.SingleValueExtendedProperties = e.SingleValueExtendedProperties;
            this.Start = e.Start;
            this.Subject = e.Subject;
            this.Type = e.Type;
            this.WebLink = e.WebLink;

            if (this.Start != null)
            {
                DateTime startDate;
                DateTime.TryParse(
                    this.Start.DateTime,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out startDate);
                this.StartDate = startDate;
            }

            if (this.End != null)
            {
                DateTime endDate;
                DateTime.TryParse(
                    this.End.DateTime,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out endDate);
                this.EndDate = endDate;
            }
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}