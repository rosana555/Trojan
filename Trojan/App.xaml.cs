using System.Configuration;
using System.Data;
using System.Windows;
using Trojan.DataBase;

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
        }
    }
}
