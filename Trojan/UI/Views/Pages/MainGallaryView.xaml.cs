using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Trojan.Core.Models;
using Trojan.Data.DataBase;
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


        private void CreateCollage()
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            var selectedImages = GalleryPanel.Children
                .OfType<GalleryImageItem>()
                .Where(i => i.IsSelected)
                .ToList();
            MessageBox.Show(
            $"Selected images: {selectedImages.Count}");

            if (selectedImages.Count == 0)
            {
                MessageBox.Show(
                    "Izberi vsaj eno sliko.",
                    "Kolaž",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            const int imageSize = 300;
            const int spacing = 10;
            const int columns = 2;

            int rows = (int)Math.Ceiling(
                selectedImages.Count / (double)columns);

            int collageWidth =
                columns * imageSize + (columns + 1) * spacing;

            int collageHeight =
                rows * imageSize + (rows + 1) * spacing;
            string collagePath;
            using (Bitmap collage = new Bitmap(
                collageWidth,
                collageHeight))
            {
                using Graphics graphics = Graphics.FromImage(collage);

                graphics.Clear(System.Drawing.Color.White);

                for (int i = 0; i < selectedImages.Count; i++)
                {
                    var item = selectedImages[i];

                    if (item.Tag is not GalleryItem galleryItem)
                        continue;

                    //MessageBox.Show(galleryItem.FilePath);
                    System.Drawing.Image image;

                    string imagePath;

                    if (Path.IsPathFullyQualified(galleryItem.FilePath))
                    {
                        imagePath = galleryItem.FilePath;
                    }
                    else
                    {
                        imagePath = Path.Combine(
                            Directory.GetParent(
                                AppDomain.CurrentDomain.BaseDirectory)!
                                .Parent!
                                .Parent!
                                .Parent!
                                .FullName,
                            galleryItem.FilePath.Replace('/', '\\'));


                    }

                    if (!File.Exists(imagePath))
                    {
                        //MessageBox.Show($"Missing image: {imagePath}");
                        continue;
                    }

                    image = System.Drawing.Image.FromFile(imagePath);

                    int row = i / columns;
                    int column = i % columns;

                    int x = spacing + column * (imageSize + spacing);
                    int y = spacing + row * (imageSize + spacing);

                    graphics.DrawImage(
                        image,
                        new Rectangle(x, y, imageSize, imageSize));
                }

                string collageDirectory = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.Desktop),
                    "Collages");

                Directory.CreateDirectory(collageDirectory);

                collagePath = Path.Combine(
                    collageDirectory,
                    $"collage_{Guid.NewGuid():N}.png");

                collage.Save(collagePath, ImageFormat.Png);
            }


            var collageItem = new GalleryItem
            {
                FilePath = collagePath,
                Description = "Kolaž",
                AddedAt = DateTime.UtcNow
            };

            DataBaseUtil.AddGalleryItem(collageItem);

            if (vm.SelectedAlbum != null)
            {
                DataBaseUtil.AddImageToAlbum(
                    vm.SelectedAlbum.Id,
                    collageItem.Id);

                vm.SelectedAlbum.Contents.Add(collageItem);
            }

            vm.GalleryItems.Add(collageItem);

            MessageBox.Show(
                "Kolaž uspešno ustvarjen!",
                "Kolaž",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            vm.IsCreatingCollage = false;

            LoadAlbums();
        }


        private void CreateCollageButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GalleryViewModel vm)
            {
                vm.IsCreatingAlbum = false;
                vm.IsCreatingCollage = true;

                GalleryTitle.Text = "Ustvarjanje kolaža";

                GalleryPanel.Children.Clear();
                foreach (var galleryItem in vm.GalleryItems)
                {
                    if (galleryItem.Description == "Kolaž")
                    {
                        AddImage(galleryItem);
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(galleryItem.OriginalFilePath) &&
                        File.Exists(galleryItem.OriginalFilePath))
                    {
                        AddImage(galleryItem);
                    }
                }
            }
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

            foreach (var galleryItem in vm.GalleryItems
                         .Where(g => g.Description == "Kolaž"))
            {
                AddImage(galleryItem);
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
                if (galleryItem.Description == "Kolaž")
                {
                    AddImage(galleryItem);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(galleryItem.OriginalFilePath) &&
                    File.Exists(galleryItem.OriginalFilePath))
                {
                    AddImage(galleryItem);
                }
            }
        }

        private void AddAlbum(Album album)
        {
            string thumbnailPath = PlaceholderAlbumImage;

            var thumbnail = album.Thumbnail
                ?? album.Contents.FirstOrDefault();

            if (thumbnail != null &&
                !string.IsNullOrWhiteSpace(thumbnail.OriginalFilePath) &&
                File.Exists(thumbnail.OriginalFilePath))
            {
                thumbnailPath = ResolveImagePath(thumbnail.FilePath);
            }

            var albumItem = new AlbumItem
            {
                Width = 220,
                Height = 260,
                AlbumTitle = album.Title,
                AlbumImage = thumbnailPath,
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
                if (galleryItem.Description == "Kolaž")
                {
                    AddImage(galleryItem);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(galleryItem.OriginalFilePath) &&
                    File.Exists(galleryItem.OriginalFilePath))
                {
                    AddImage(galleryItem);
                }
            }
        }
        private void ImageItem_DeleteRequested(object? sender, EventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            if (vm.SelectedAlbum == null)
                return;

            if (sender is not GalleryImageItem imageItem ||
                imageItem.Tag is not GalleryItem galleryItem)
                return;

            var result = MessageBox.Show(
                "Odstranim sliko iz albuma?",
                "Potrditev",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;
            MessageBox.Show(
    $"Brisem sliko {galleryItem.Id} iz albuma {vm.SelectedAlbum.Id}");

            DataBaseUtil.RemoveImageFromAlbum(
                vm.SelectedAlbum.Id,
                galleryItem.Id);


            vm.ReloadData(selectedAlbumId: vm.SelectedAlbum.Id);


            if (vm.SelectedAlbum != null)
            {
                OpenAlbum(vm.SelectedAlbum);
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

            imageItem.DeleteRequested += ImageItem_DeleteRequested;

            GalleryPanel.Children.Add(imageItem);
        }

        private void ImageItem_MouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            // Navaden ogled slike
            if (!vm.IsCreatingAlbum && !vm.IsCreatingCollage)
            {
                if (sender is GalleryImageItem imageItem &&
                    imageItem.Tag is GalleryItem galleryItem)
                {
                    var viewer =
                        new ImageViewerWindow(galleryItem.FilePath);

                    viewer.ShowDialog();
                }

                return;
            }

            if (sender is not GalleryImageItem clickedImage)
                return;

            // THUMBNAIL SELECTION MODE
            if (_isSelectingThumbnail)
            {
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

            if (clickedImage.IsThumbnail)
            {
                clickedImage.IsThumbnail = false;

                if (_thumbnailImage == clickedImage)
                {
                    _thumbnailImage = null;
                }

                return;
            }

            clickedImage.IsSelected = !clickedImage.IsSelected;
        }
        private void SelectThumbnailButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not GalleryViewModel vm)
                return;

            if (!vm.IsCreatingAlbum &&
                !vm.IsCreatingCollage)
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
                if (vm.IsCreatingCollage)
                {
                    vm.IsCreatingCollage = false;
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
                !vm.IsCreatingCollage &&
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
            // SAVE COLLAGE
            if (vm.IsCreatingCollage)
            {
                CreateCollage();
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
            {
                if (File.Exists(path))
                    return path;

                return PlaceholderAlbumImage;
            }

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