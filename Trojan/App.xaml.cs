using System.Windows;
using Trojan.DataBase;
using Trojan.Views;

namespace Trojan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
