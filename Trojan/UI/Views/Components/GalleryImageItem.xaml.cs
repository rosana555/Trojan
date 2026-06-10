using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Trojan.Services.Logger;

namespace Trojan.UI.Views.Components
{
    public partial class GalleryImageItem : UserControl
    {
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register(
                nameof(ImagePath),
                typeof(string),
                typeof(GalleryImageItem),
                new PropertyMetadata(null, OnImagePathChanged));

        public string ImageDate
        {
            get { return (string)GetValue(ImageDateProperty); }
            set { SetValue(ImageDateProperty, value); }
        }

        public static readonly DependencyProperty ImageDateProperty =
            DependencyProperty.Register(
                nameof(ImageDate),
                typeof(string),
                typeof(GalleryImageItem),
                new PropertyMetadata(null, OnImageDateChanged));

        // SELECTED
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(GalleryImageItem),
                new PropertyMetadata(false, OnSelectionChanged));

        // THUMBNAIL
        public bool IsThumbnail
        {
            get { return (bool)GetValue(IsThumbnailProperty); }
            set { SetValue(IsThumbnailProperty, value); }
        }

        public static readonly DependencyProperty IsThumbnailProperty =
            DependencyProperty.Register(
                nameof(IsThumbnail),
                typeof(bool),
                typeof(GalleryImageItem),
                new PropertyMetadata(false, OnSelectionChanged));

        public GalleryImageItem()
        {
            InitializeComponent();
        }

        private static void OnImagePathChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (GalleryImageItem)d;

            if (e.NewValue is not string path ||
                string.IsNullOrWhiteSpace(path))
            {
                control.ImageSource.Source = null;
                return;
            }

            var uriKind = Path.IsPathFullyQualified(path)
                ? UriKind.Absolute
                : UriKind.Relative;

            control.ImageSource.Source =
                new BitmapImage(new Uri(path, uriKind));
        }

        private static void OnImageDateChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (GalleryImageItem)d;

            control.DateText.Text =
                e.NewValue?.ToString() ?? "";
        }

        private static void OnSelectionChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (GalleryImageItem)d;

            // ORANGE = THUMBNAIL
            if (control.IsThumbnail)
            {
                control.RootBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(255, 232, 204));

                return;
            }

            // BLUE = SELECTED
            if (control.IsSelected)
            {
                control.RootBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(220, 238, 255));

                return;
            }

            // DEFAULT
            control.RootBorder.Background =
                Brushes.White;
        }

        private void Grid_MouseDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            AppLog.Info(
                $"Kliknil na sliko: {ImagePath}, datum: {ImageDate}");
        }

        public event EventHandler? DeleteRequested;
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }
    }


}