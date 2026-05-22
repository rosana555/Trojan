using System.Windows;
using System.Windows.Controls;
using Trojan.Services.Logger;

namespace Trojan.UI.Views
{
    public partial class GalleryImageItem : UserControl
    {
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(GalleryImageItem), 
                new PropertyMetadata(null, OnImagePathChanged));

        public string ImageDate
        {
            get { return (string)GetValue(ImageDateProperty); }
            set { SetValue(ImageDateProperty, value); }
        }

        public static readonly DependencyProperty ImageDateProperty =
            DependencyProperty.Register("ImageDate", typeof(string), typeof(GalleryImageItem), 
                new PropertyMetadata(null, OnImageDateChanged));

        public GalleryImageItem()
        {
            InitializeComponent();
        }

        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GalleryImageItem)d;
            if (e.NewValue is string path && !string.IsNullOrEmpty(path))
            {
                control.ImageSource.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri(path, System.UriKind.Absolute));
            }
        }

        private static void OnImageDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GalleryImageItem)d;
            control.DateText.Text = e.NewValue?.ToString() ?? "";
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AppLog.Info($"Kliknil na sliko: {ImagePath}, datum: {ImageDate}");
        }
    }
}
