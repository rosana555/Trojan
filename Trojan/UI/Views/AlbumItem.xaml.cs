using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Trojan.UI.Views
{
    public partial class AlbumItem : UserControl
    {
        public AlbumItem()
        {
            InitializeComponent();
        }

        public string AlbumTitle
        {
            get => (string)GetValue(AlbumTitleProperty);
            set => SetValue(AlbumTitleProperty, value);
        }

        public static readonly DependencyProperty AlbumTitleProperty =
            DependencyProperty.Register(
                nameof(AlbumTitle),
                typeof(string),
                typeof(AlbumItem),
                new PropertyMetadata("", OnTitleChanged));

        private static void OnTitleChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((AlbumItem)d).TitleText.Text = e.NewValue?.ToString();
        }

        public string AlbumImage
        {
            get => (string)GetValue(AlbumImageProperty);
            set => SetValue(AlbumImageProperty, value);
        }

        public static readonly DependencyProperty AlbumImageProperty =
            DependencyProperty.Register(
                nameof(AlbumImage),
                typeof(string),
                typeof(AlbumItem),
                new PropertyMetadata("", OnImageChanged));

        private static void OnImageChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not string path)
                return;

            ((AlbumItem)d).AlbumImageControl.Source =
                new BitmapImage(new System.Uri(path));
        }

        public int ImageCount
        {
            get => (int)GetValue(ImageCountProperty);
            set => SetValue(ImageCountProperty, value);
        }

        public static readonly DependencyProperty ImageCountProperty =
            DependencyProperty.Register(
                nameof(ImageCount),
                typeof(int),
                typeof(AlbumItem),
                new PropertyMetadata(0, OnCountChanged));

        private static void OnCountChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((AlbumItem)d).CountText.Text =
                $"Slike: {e.NewValue}";
        }
    }
}