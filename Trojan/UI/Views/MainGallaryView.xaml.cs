using System.Windows;
using System.Windows.Controls;
using Trojan.Services.Logger;
using Trojan.UI.ViewModels;

namespace Trojan.UI.Views
{
    public partial class MainGallaryView : UserControl
    {
        private bool _isInsideAlbum = false;

        public MainGallaryView()
        {
            InitializeComponent();

            Loaded += MainGallaryView_Loaded;
        }

        private void MainGallaryView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAlbums();
        }

        private void LoadAlbums()
        {
            _isInsideAlbum = false;

            GalleryTitle.Text = "Albums";

            GalleryPanel.Children.Clear();

            AddAlbum(
                "Vacation",
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image1.jpg",
                12);

            AddAlbum(
                "Nature",
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image2.jpg",
                8);

            AddAlbum(
                "Family",
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image3.jpg",
                16);

            AddAlbum(
                "Work",
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image4.jpg",
                5);
        }

        private void AddAlbum(string title, string image, int count)
        {
            var album = new AlbumItem
            {
                Width = 220,
                Height = 260,
                AlbumTitle = title,
                AlbumImage = image,
                ImageCount = count,
                Margin = new Thickness(10)
            };

            album.MouseDoubleClick += Album_MouseDoubleClick;

            GalleryPanel.Children.Add(album);
        }

        private void Album_MouseDoubleClick(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not AlbumItem album)
                return;

            AppLog.Info($"Opened album: {album.AlbumTitle}");

            OpenAlbum(album.AlbumTitle);
        }

        private void OpenAlbum(string title)
        {
            _isInsideAlbum = true;

            GalleryTitle.Text = title;

            GalleryPanel.Children.Clear();

            AddImage(
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image1.jpg",
                "datum: 1.8.1924");

            AddImage(
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image2.jpg",
                "datum: 4.8.1924");

            AddImage(
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image3.jpg",
                "datum: 12.8.1924");

            AddImage(
                @"C:\Users\Uporabnik\source\repos\Trojan\Trojan\UI\Assets\TestImages\image4.jpg",
                "datum: 17.8.1924");
        }

        private void AddImage(string path, string date)
        {
            GalleryPanel.Children.Add(
                new GalleryImageItem
                {
                    Width = 220,
                    Height = 220,
                    Margin = new Thickness(10),
                    ImagePath = path,
                    ImageDate = date
                });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isInsideAlbum)
            {
                LoadAlbums();
                return;
            }

            if (DataContext is HelperOverlayViewModel vm)
            {
                vm.OpenGallaryCommand.Execute(null);
            }
        }
    }
}