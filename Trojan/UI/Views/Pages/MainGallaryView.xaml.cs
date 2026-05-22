using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Trojan.Services.Logger;
using Trojan.UI.ViewModels;
using Trojan.UI.Views.Components;
using System.Windows.Media;
namespace Trojan.UI.Views.Pages
{
    public partial class MainGallaryView : UserControl
    {
        private GalleryImageItem? _selectedImage;

        private GalleryImageItem? _thumbnailImage;
        private bool _isSelectingThumbnail;
        public MainGallaryView()
        {
            InitializeComponent();

            Loaded += MainGallaryView_Loaded;
        }

        private void MainGallaryView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GalleryViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }

            LoadAlbums();
        }

        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            if (e.PropertyName == nameof(vm.IsCreatingAlbum))
            {
                if (vm.IsCreatingAlbum)
                {
                    LoadCreateAlbumMode();
                }
                else
                {
                    LoadAlbums();
                }
            }
        }

        private void LoadAlbums()
        {
            var vm = DataContext as GalleryViewModel;

            if (vm == null)
                return;

            vm.IsInsideAlbum = false;

            GalleryTitle.Text = "Albums";

            GalleryPanel.Children.Clear();

            AddAlbum(
                "Vacation",
                "/Trojan;component/UI/Assets/TestImages/image1.jpg",
                12);

            AddAlbum(
                "Nature",
                "/Trojan;component/UI/Assets/TestImages/image2.jpg",
                8);

            AddAlbum(
                "Family",
                "/Trojan;component/UI/Assets/TestImages/image3.jpg",
                16);

            AddAlbum(
                "Work",
                "/Trojan;component/UI/Assets/TestImages/image4.jpg",
                5);
        }

        private void LoadCreateAlbumMode()
        {
            GalleryTitle.Text = "Ustvarjanje albuma";

            GalleryPanel.Children.Clear();

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image1.jpg",
                "datum: 1.8.1924");

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image2.jpg",
                "datum: 4.8.1924");

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image3.jpg",
                "datum: 12.8.1924");

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image4.jpg",
                "datum: 17.8.1924");
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

        private void Album_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not AlbumItem album)
                return;

            AppLog.Info(
                $"Opened album: {album.AlbumTitle}");

            OpenAlbum(album.AlbumTitle);
        }

        private void OpenAlbum(string title)
        {
            if (DataContext is GalleryViewModel vm)
            {
                vm.IsInsideAlbum = true;
            }

            GalleryTitle.Text = title;

            GalleryPanel.Children.Clear();

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image1.jpg",
                "datum: 1.8.1924");

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image2.jpg",
                "datum: 4.8.1924");

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image3.jpg",
                "datum: 12.8.1924");

            AddImage(
                "/Trojan;component/UI/Assets/TestImages/image4.jpg",
                "datum: 17.8.1924");
        }

        private void AddImage(string path, string date)
        {
            var imageItem = new GalleryImageItem
            {
                Width = 220,
                Height = 220,
                Margin = new Thickness(10),
                ImagePath = path,
                ImageDate = date
            };

            imageItem.MouseLeftButtonDown +=
                ImageItem_MouseLeftButtonDown;

            GalleryPanel.Children.Add(imageItem);
        }

        private void ImageItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            if (!vm.IsCreatingAlbum)
                return;

            if (sender is not GalleryImageItem clickedImage)
                return;

            // THUMBNAIL SELECTION MODE
            if (_isSelectingThumbnail)
            {
                // remove old thumbnail
                if (_thumbnailImage != null)
                {
                    _thumbnailImage.IsThumbnail = false;
                }

                _thumbnailImage = clickedImage;

                clickedImage.IsSelected = false;
                clickedImage.IsThumbnail = true;

                _isSelectingThumbnail = false;

                return;
            }

            // thumbnail click = remove thumbnail
            if (clickedImage.IsThumbnail)
            {
                clickedImage.IsThumbnail = false;

                if (_thumbnailImage == clickedImage)
                {
                    _thumbnailImage = null;
                }

                return;
            }

            // NORMAL TOGGLE
            clickedImage.IsSelected =
                !clickedImage.IsSelected;
        }
        private void SelectThumbnailButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            if (!vm.IsCreatingAlbum)
                return;

            _isSelectingThumbnail = true;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GalleryViewModel vm)
            {
                if (vm.IsCreatingAlbum)
                {
                    vm.IsCreatingAlbum = false;
                    LoadAlbums();
                    return;
                }

                if (vm.IsInsideAlbum)
                {
                    LoadAlbums();
                    return;
                }

                if (Window.GetWindow(this)?.DataContext
                    is HelperOverlayViewModel helperVm)
                {
                    helperVm.IsGallaryVisible = false;
                }
            }
        }

        private void MainActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            // ENTER CREATE MODE
            if (!vm.IsCreatingAlbum &&
                !vm.IsInsideAlbum)
            {
                vm.IsCreatingAlbum = true;
                return;
            }

            // SAVE
            if (vm.IsCreatingAlbum)
            {
                vm.IsCreatingAlbum = false;

                LoadAlbums();

                return;
            }

            // ADD IMAGE
            if (vm.IsInsideAlbum)
            {
                vm.IsAddingImage = true;
            }
        }
        private void CloseAddImageForm_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (DataContext is GalleryViewModel vm)
            {
                vm.IsAddingImage = false;
            }
        }
    }
}