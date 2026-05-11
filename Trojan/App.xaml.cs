using System.Windows;
using Trojan.DataBase;
using Trojan.Views;
using Trojan.Services;

namespace Trojan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await DeviceScannerService.SaveDeviceInfo();

            //PODATKE O NAPRAVI SE SHRAANIJO NA "C:\Users\IME\AppData\Roaming\gnezdece\device_info.txt"

            // Ob zagonu aplikacije zagotovimo, da je podatkovna baza ustvarjena
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            var helperOverlayWindow = new HelperOverlayWindow();
            MainWindow = helperOverlayWindow;
            helperOverlayWindow.Show();
        }
    }
}
