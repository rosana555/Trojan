using System.Windows;
using Trojan.DataBase;
using Trojan.Seeders;
using Trojan.Views;

namespace Trojan
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            System.Diagnostics.Debug.WriteLine("🔥 APP STARTED");

            new AppBootstrapper().Run();

            var mainWindow = new HelperOverlayWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}