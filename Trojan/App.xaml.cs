using System.Windows;
using Trojan.Data.DataBase;
using Trojan.Data.Seeders;
using Trojan.Views;
using Trojan.Services;

namespace Trojan
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
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