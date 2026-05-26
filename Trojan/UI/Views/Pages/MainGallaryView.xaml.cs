using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Trojan.Core.Models;
using Trojan.Services.Logger;
using Trojan.UI.ViewModels;
using Trojan.UI.Views.Components;

namespace Trojan.UI.Views.Pages
{
    public partial class MainGallaryView : UserControl
    {
        private const string PlaceholderAlbumImage =
            "/Trojan;component/UI/Assets/TestImages/image1.jpg";

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
                vm.PropertyChanged -= Vm_PropertyChanged;
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
            vm.SelectedAlbum = null;

            GalleryTitle.Text = "Albums";

            GalleryPanel.Children.Clear();

            foreach (var album in vm.Albums)
            {
                AddAlbum(album);
            }
        }

        private void LoadCreateAlbumMode()
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            GalleryTitle.Text = "Ustvarjanje albuma";

            GalleryPanel.Children.Clear();

            foreach (var galleryItem in vm.GalleryItems)
            {
                AddImage(galleryItem);
            }
        }

        private void AddAlbum(Album album)
        {
            var albumItem = new AlbumItem
            {
                Width = 220,
                Height = 260,
                AlbumTitle = album.Title,
                AlbumImage = ResolveImagePath(
                    album.Thumbnail?.FilePath
                    ?? album.Contents.FirstOrDefault()?.FilePath
                    ?? PlaceholderAlbumImage),
                ImageCount = album.Contents.Count,
                Tag = album,
                Margin = new Thickness(10)
            };

            albumItem.MouseDoubleClick += Album_MouseDoubleClick;

            GalleryPanel.Children.Add(albumItem);
        }

        private void Album_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not AlbumItem albumItem ||
                albumItem.Tag is not Album album)
                return;

            AppLog.Info(
                $"Opened album: {album.Title}");

            OpenAlbum(album);
        }

        private void OpenAlbum(Album album)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            var selectedAlbum = vm.Albums.FirstOrDefault(
                a => a.Id == album.Id) ?? album;

            vm.IsInsideAlbum = true;
            vm.SelectedAlbum = selectedAlbum;

            GalleryTitle.Text = selectedAlbum.Title;

            GalleryPanel.Children.Clear();

            foreach (var galleryItem in selectedAlbum.Contents)
            {
                AddImage(galleryItem);
            }
        }

        private void AddImage(GalleryItem galleryItem)
        {
            var imageItem = new GalleryImageItem
            {
                Width = 220,
                Height = 220,
                Margin = new Thickness(10),
                ImagePath = ResolveImagePath(galleryItem.FilePath),
                ImageDate = $"datum: {galleryItem.AddedAt.ToLocalTime():d.M.yyyy}",
                Tag = galleryItem
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
                vm.SaveAlbumCommand.Execute(null);
                LoadAlbums();
                return;
            }

            // ADD IMAGE
            if (vm.IsInsideAlbum)
            {
                vm.SetPendingImagePath(string.Empty);
                vm.PendingImageDescription = string.Empty;
                vm.IsAddingImage = true;
            }
        }
        private void CloseAddImageForm_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (DataContext is GalleryViewModel vm)
            {
                vm.CancelAddImageCommand.Execute(null);
            }
        }

        private void UploadImageButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                vm.SetPendingImagePath(dialog.FileName);
            }
        }

        private void SaveAddImageButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            try
            {
                if (!vm.TrySavePendingImage())
                {
                    MessageBox.Show(
                        "Najprej izberi sliko za nalaganje.",
                        "Upload image",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }

                if (vm.SelectedAlbum != null)
                {
                    OpenAlbum(vm.SelectedAlbum);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Shranjevanje slike ni uspelo: {ex.Message}",
                    "Upload image",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private static string ResolveImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return PlaceholderAlbumImage;

            if (Path.IsPathFullyQualified(path))
                return path;

            if (path.StartsWith(
                    "/Trojan;component/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            var normalizedPath = path.Replace('\\', '/').TrimStart('/');

            return $"/Trojan;component/{normalizedPath}";
        }
    }
}