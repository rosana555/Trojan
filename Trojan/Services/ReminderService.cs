using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Trojan.Core.Models;
using Trojan.Data.DataBase;

namespace Trojan.Services
{
    public class ReminderService
    {
        private readonly DispatcherTimer _timer;

        public ReminderService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _timer.Tick += CheckReminders;
        }

        public void Start() => _timer.Start();
        public void Stop()  => _timer.Stop();

        private void CheckReminders(object sender, EventArgs e)
        {
            var now = DateTime.UtcNow;

            using var db = new AppDbContext();
            var due = db.Reminders
                .Where(r => !r.IsTriggered && r.TriggerAt <= now)
                .ToList();

            foreach (var reminder in due)
            {
                // Prikaži obvestilo
                MessageBox.Show(
                    $"{reminder.Title}",
                    "Opomnik",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Ponavljajoči opomnki — nastavi naslednji čas
                if (reminder.Recurrence == RecurrenceType.Daily)
                    reminder.TriggerAt = reminder.TriggerAt.AddDays(1);
                else if (reminder.Recurrence == RecurrenceType.Weekly)
                    reminder.TriggerAt = reminder.TriggerAt.AddDays(7);
                else if (reminder.Recurrence == RecurrenceType.Monthly)
                    reminder.TriggerAt = reminder.TriggerAt.AddMonths(1);
                else
                    reminder.IsTriggered = true;
            }

            db.SaveChanges();
        }
    }
}