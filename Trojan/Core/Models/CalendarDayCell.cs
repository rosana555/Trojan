using System;
using System.Collections.ObjectModel;

namespace Trojan.Core.Models
{
    public class CalendarDayCell
    {
        public DateTime Date { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsToday { get; set; }
        public ObservableCollection<CalendarEvent> Events { get; set; } = new();
    }
}