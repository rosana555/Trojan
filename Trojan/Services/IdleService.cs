using System.Windows.Threading;

public sealed class IdleService
{
    private readonly DispatcherTimer _timer;

    private DateTime _lastActivity = DateTime.Now;

    public event Action? SleepRequested;
    public event Action? WakeRequested;

    private bool _sleeping;

    public IdleService()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += OnTick;
        _timer.Start();
    }

    public void RegisterActivity()
    {
        _lastActivity = DateTime.Now;

        if (_sleeping)
        {
            _sleeping = false;
            WakeRequested?.Invoke();
        }
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var idle = DateTime.Now - _lastActivity;

        if (!_sleeping && idle >= TimeSpan.FromSeconds(30))
        {
            _sleeping = true;
            SleepRequested?.Invoke();
        }

        if (_sleeping && idle >= TimeSpan.FromSeconds(60))
        {
            _sleeping = false;

            _lastActivity = DateTime.Now;

            WakeRequested?.Invoke();
        }
    }
}