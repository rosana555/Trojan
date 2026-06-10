using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Trojan.UI.Views.Pages
{
    public partial class ImageViewerWindow : Window
    {
        public ImageViewerWindow(string imagePath)
        {
            InitializeComponent();

            PreviewImage.Source =
                new BitmapImage(new Uri(imagePath));
        }
    }
}