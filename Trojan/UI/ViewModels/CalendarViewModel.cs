using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Trojan.Core.Base;
using Trojan.Core.Commands;
using Trojan.Core.Models;
using Trojan.Data.DataBase;

namespace Trojan.UI.ViewModels
{
    public class CalendarViewModel : ObservableObject
    {
        // - Navigation
        private DateTime _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime CurrentMonth
        {
            get => _currentMonth;
            set
            {
                SetProperty(ref _currentMonth, value);
                OnPropertyChanged(nameof(MonthLabel));
                BuildMonthGrid();
            }
        }

        public string MonthLabel => CurrentMonth.ToString("MMMM yyyy");

        // - View mode
        private bool _isMonthView = true;
        public bool IsMonthView
        {
            get => _isMonthView;
            set
            {
                SetProperty(ref _isMonthView, value);
                OnPropertyChanged(nameof(IsDayView));
            }
        }
        public bool IsDayView => !_isMonthView;

        // - Selected day
        private DateTime _selectedDay = DateTime.Today;
        public DateTime SelectedDay
        {
            get => _selectedDay;
            set
            {
                SetProperty(ref _selectedDay, value);
                OnPropertyChanged(nameof(DayLabel));
                LoadDayEvents();
            }
        }

        public string DayLabel => SelectedDay.ToString("dddd, dd. MMMM yyyy");

        // - Collections
        public ObservableCollection<CalendarDayCell> MonthCells { get; } = new();
        public ObservableCollection<CalendarEvent> DayEvents { get; } = new();

        // - Event form
        private CalendarEvent _editingEvent = new();
        public CalendarEvent EditingEvent
        {
            get => _editingEvent;
            set
            {
                SetProperty(ref _editingEvent, value);
                OnPropertyChanged(nameof(StartHour));
                OnPropertyChanged(nameof(StartMinute));
                OnPropertyChanged(nameof(EndHour));
                OnPropertyChanged(nameof(EndMinute));
            }
        }

        private bool _isFormOpen;
        public bool IsFormOpen
        {
            get => _isFormOpen;
            set => SetProperty(ref _isFormOpen, value);
        }

        // - Reminder form
        private Reminder _editingReminder = new();
        public Reminder EditingReminder
        {
            get => _editingReminder;
            set => SetProperty(ref _editingReminder, value);
        }

        private bool _isReminderFormOpen;
        public bool IsReminderFormOpen
        {
            get => _isReminderFormOpen;
            set => SetProperty(ref _isReminderFormOpen, value);
        }

        // - Commands
        public ICommand PrevMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand SwitchViewCommand { get; }
        public ICommand OpenAddEventCommand { get; }
        public ICommand SaveEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand CloseFormCommand { get; }
        public ICommand OpenReminderCommand { get; }
        public ICommand SaveReminderCommand { get; }
        public ICommand SelectDayCommand { get; }
        public ICommand EditEventCommand { get; }
        public ICommand PrevDayCommand { get; }
        public ICommand NextDayCommand { get; }

        public CalendarViewModel()
        {
            PrevMonthCommand    = new RelayCommand(() =>
            {
                CurrentMonth = CurrentMonth.AddMonths(-1);
                if (!IsMonthView)
                    SelectedDay = CurrentMonth;
            });
            NextMonthCommand    = new RelayCommand(() =>
            {
                CurrentMonth = CurrentMonth.AddMonths(1);
                if (!IsMonthView)
                    SelectedDay = CurrentMonth;
            });
            SwitchViewCommand   = new RelayCommand(() => IsMonthView = !IsMonthView);
            OpenAddEventCommand = new RelayCommand(OpenNewEventForm);
            SaveEventCommand    = new RelayCommand(SaveEvent);
            DeleteEventCommand  = new RelayCommand<CalendarEvent>(DeleteEvent);
            EditEventCommand    = new RelayCommand<CalendarEvent>(OpenEditForm);
            CloseFormCommand    = new RelayCommand(() =>
            {
                IsFormOpen = false;
                IsReminderFormOpen = false;
            });
            OpenReminderCommand = new RelayCommand(() =>
            {
                EditingReminder = new Reminder { TriggerAt = DateTime.Now.AddHours(1) };
                IsReminderFormOpen = true;
            });
            SaveReminderCommand = new RelayCommand(SaveReminder);
            SelectDayCommand    = new RelayCommand<DateTime>(day =>
            {
                SelectedDay = day;
                IsMonthView = false;
            });
            PrevDayCommand = new RelayCommand(() =>
            {
                SelectedDay = SelectedDay.AddDays(-1);
                SyncMonthToDay();
            });
            NextDayCommand = new RelayCommand(() =>
            {
                SelectedDay = SelectedDay.AddDays(1);
                SyncMonthToDay();
            });

            BuildMonthGrid();
        }

        private void BuildMonthGrid()
        {
            MonthCells.Clear();

            int startDow = ((int)CurrentMonth.DayOfWeek + 6) % 7;
            for (int i = 0; i < startDow; i++)
                MonthCells.Add(new CalendarDayCell { IsEmpty = true });

            int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);

            using var db = new AppDbContext();
            var events = db.CalendarEvents
                .Where(e => e.StartDateTime.Year  == CurrentMonth.Year &&
                            e.StartDateTime.Month == CurrentMonth.Month)
                .ToList();

            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = new DateTime(CurrentMonth.Year, CurrentMonth.Month, d);
                MonthCells.Add(new CalendarDayCell
                {
                    Date    = date,
                    IsToday = date.Date == DateTime.Today,
                    Events  = new ObservableCollection<CalendarEvent>(
                                events.Where(e => e.StartDateTime.Date == date.Date))
                });
            }
        }

        private void LoadDayEvents()
        {
            DayEvents.Clear();
            using var db = new AppDbContext();
            var evs = db.CalendarEvents
                .Where(e => e.StartDateTime.Date == SelectedDay.Date)
                .OrderBy(e => e.StartDateTime)
                .ToList();
            foreach (var ev in evs)
                DayEvents.Add(ev);
        }

        private void OpenNewEventForm()
        {
            EditingEvent = new CalendarEvent
            {
                StartDateTime = SelectedDay.Date.AddHours(9),
                EndDateTime   = SelectedDay.Date.AddHours(10),
                ColorHex      = "#A13599"
            };
            IsFormOpen = true;
        }

        private void OpenEditForm(CalendarEvent ev)
        {
            if (ev == null) return;
            EditingEvent = ev;
            IsFormOpen = true;
        }

        private void SaveEvent()
        {
            using var db = new AppDbContext();
            if (EditingEvent.Id == 0)
                db.CalendarEvents.Add(EditingEvent);
            else
                db.CalendarEvents.Update(EditingEvent);
            db.SaveChanges();

            IsFormOpen = false;
            BuildMonthGrid();
            LoadDayEvents();
        }

        private void DeleteEvent(CalendarEvent ev)
        {
            if (ev == null) return;
            using var db = new AppDbContext();
            db.CalendarEvents.Remove(ev);
            db.SaveChanges();
            BuildMonthGrid();
            LoadDayEvents();
        }

        private void SaveReminder()
        {
            using var db = new AppDbContext();
            db.Reminders.Add(EditingReminder);
            db.SaveChanges();
            IsReminderFormOpen = false;
        }

        private void SyncMonthToDay()
        {
            var firstOfMonth = new DateTime(SelectedDay.Year, SelectedDay.Month, 1);
            if (firstOfMonth != CurrentMonth)
                CurrentMonth = firstOfMonth;
        }

        // ── Time helpers – Event ────────────────────────────────────────────

        public string StartHour
        {
            get => EditingEvent.StartDateTime.Hour.ToString("D2");
            set
            {
                if (int.TryParse(value, out int h) && h >= 0 && h <= 23)
                {
                    var dt = EditingEvent.StartDateTime;
                    EditingEvent.StartDateTime = new DateTime(dt.Year, dt.Month, dt.Day, h, dt.Minute, 0);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EditingEvent));
                }
            }
        }

        public string StartMinute
        {
            get => EditingEvent.StartDateTime.Minute.ToString("D2");
            set
            {
                if (int.TryParse(value, out int m) && m >= 0 && m <= 59)
                {
                    var dt = EditingEvent.StartDateTime;
                    EditingEvent.StartDateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, m, 0);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EditingEvent));
                }
            }
        }

        public string EndHour
        {
            get => EditingEvent.EndDateTime.Hour.ToString("D2");
            set
            {
                if (int.TryParse(value, out int h) && h >= 0 && h <= 23)
                {
                    var dt = EditingEvent.EndDateTime;
                    EditingEvent.EndDateTime = new DateTime(dt.Year, dt.Month, dt.Day, h, dt.Minute, 0);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EditingEvent));
                }
            }
        }

        public string EndMinute
        {
            get => EditingEvent.EndDateTime.Minute.ToString("D2");
            set
            {
                if (int.TryParse(value, out int m) && m >= 0 && m <= 59)
                {
                    var dt = EditingEvent.EndDateTime;
                    EditingEvent.EndDateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, m, 0);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EditingEvent));
                }
            }
        }

        // ── Time helpers – Reminder ─────────────────────────────────────────

        public string ReminderHour
        {
            get => EditingReminder.TriggerAt.Hour.ToString("D2");
            set
            {
                if (int.TryParse(value, out int h) && h >= 0 && h <= 23)
                {
                    var dt = EditingReminder.TriggerAt;
                    EditingReminder.TriggerAt = new DateTime(dt.Year, dt.Month, dt.Day, h, dt.Minute, 0);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EditingReminder));
                }
            }
        }

        public string ReminderMinute
        {
            get => EditingReminder.TriggerAt.Minute.ToString("D2");
            set
            {
                if (int.TryParse(value, out int m) && m >= 0 && m <= 59)
                {
                    var dt = EditingReminder.TriggerAt;
                    EditingReminder.TriggerAt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, m, 0);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EditingReminder));
                }
            }
        }
    }
}