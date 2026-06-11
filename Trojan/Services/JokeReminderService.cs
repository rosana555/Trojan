using System.Windows.Threading;
using Trojan.Core.Base;

namespace Trojan.Services;

public sealed class JokeReminderService : ObservableObject
{
    public static TimeSpan ReminderInterval => App._devMode ? TimeSpan.FromSeconds(30) : TimeSpan.FromMinutes(30);

    private readonly DispatcherTimer _timer;
    private bool _isReminderActive;

    public event EventHandler? ReminderActivated;

    public bool IsReminderActive
    {
        get => _isReminderActive;
        private set => SetProperty(ref _isReminderActive, value);
    }

    public JokeReminderService()
    {
        _timer = new DispatcherTimer();
        _timer.Tick += OnTimerTick;
    }

    public void Start()
    {
        _timer.Stop();
        IsReminderActive = false;
        _timer.Interval = ReminderInterval;
        _timer.Start();
    }

    public void Dismiss()
    {
        IsReminderActive = false;
        Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _timer.Stop();
        IsReminderActive = true;
        ReminderActivated?.Invoke(this, EventArgs.Empty);
    }
}
