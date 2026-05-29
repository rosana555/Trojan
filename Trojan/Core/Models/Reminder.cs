using System;

namespace Trojan.Core.Models
{
    public enum RecurrenceType { None, Daily, Weekly, Monthly }

    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime TriggerAt { get; set; }
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public int? LinkedEventId { get; set; }
        public bool IsTriggered { get; set; } = false;
    }
}