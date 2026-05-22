using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trojan.UI.ViewModels;
using Trojan.Services.Logger;

namespace Trojan.UI.Views
{
    /// <summary>
    /// Interaction logic for MainGallaryView.xaml
    /// </summary>
    public partial class MainGallaryView : UserControl
    {
        public MainGallaryView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AppLog.Info("MainGallaryView Loaded");
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AppLog.Info($"MainGallaryView DataContextChanged: {e.NewValue?.GetType().Name}");
        }
    }
}
