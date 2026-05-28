using System.Threading.Tasks;
using System.Windows;

namespace Trojan.Views
{
    public partial class BlackScreenWindow : Window
    {
        public BlackScreenWindow()
        {
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                await Task.Delay(2000); 
                Close();
            };
        }
    }
}