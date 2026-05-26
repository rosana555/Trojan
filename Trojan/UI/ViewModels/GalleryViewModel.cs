using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Trojan.Core.Base;
using Trojan.Core.Commands;
using Trojan.Core.Models;
using Trojan.Data.DataBase;

namespace Trojan.UI.ViewModels
{
    public class GalleryViewModel : ObservableObject
    {
        private static readonly string[] SupportedImageExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp"
        };

        private bool _isInsideAlbum;

        public bool IsInsideAlbum
        {
            get => _isInsideAlbum;
            set => SetProperty(ref _isInsideAlbum, value);
        }
        private bool _isSelectingThumbnail;

        public bool IsSelectingThumbnail
        {
            get => _isSelectingThumbnail;
            set => SetProperty(ref _isSelectingThumbnail, value);
        }
        private bool _isCreatingAlbum;

        public bool IsCreatingAlbum
        {
            get => _isCreatingAlbum;
            set => SetProperty(ref _isCreatingAlbum, value);
        }

        private string _newAlbumTitle = string.Empty;

        public string NewAlbumTitle
        {
            get => _newAlbumTitle;
            set => SetProperty(ref _newAlbumTitle, value);
        }
        private bool _isAddingImage;

        public bool IsAddingImage
        {
            get => _isAddingImage;
            set => SetProperty(ref _isAddingImage, value);
        }

        private string _pendingImagePath = string.Empty;

        public string PendingImagePath
        {
            get => _pendingImagePath;
            set => SetProperty(ref _pendingImagePath, value);
        }

        private string _pendingImageDescription = string.Empty;

        public string PendingImageDescription
        {
            get => _pendingImageDescription;
            set => SetProperty(ref _pendingImageDescription, value);
        }

        private ObservableCollection<GalleryItem> _galleryItems =
            new ObservableCollection<GalleryItem>();

        public ObservableCollection<GalleryItem> GalleryItems
        {
            get => _galleryItems;
            set => SetProperty(ref _galleryItems, value);
        }

        private ObservableCollection<Album> _albums =
            new ObservableCollection<Album>();

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set => SetProperty(ref _albums, value);
        }

        private Album? _selectedAlbum;

        public Album? SelectedAlbum
        {
            get => _selectedAlbum;
            set => SetProperty(ref _selectedAlbum, value);
        }

        private GalleryItem? _selectedGalleryItem;

        public GalleryItem? SelectedGalleryItem
        {
            get => _selectedGalleryItem;
            set => SetProperty(ref _selectedGalleryItem, value);
        }

        public ICommand CreateAlbumCommand { get; }

        public ICommand DeleteAlbumCommand { get; }

        public ICommand AddImageToAlbumCommand { get; }

        public ICommand RemoveImageFromAlbumCommand { get; }

        public ICommand EnterCreateAlbumModeCommand { get; }

        public ICommand SaveAlbumCommand { get; }

        public ICommand SaveUploadedImageCommand { get; }

        public ICommand CancelAddImageCommand { get; }

        public GalleryViewModel()
        {
            ReloadData();

            CreateAlbumCommand =
                new RelayCommand(CreateAlbum);

            DeleteAlbumCommand =
                new RelayCommand(DeleteAlbum);

            AddImageToAlbumCommand =
                new RelayCommand(AddImageToAlbum);

            RemoveImageFromAlbumCommand =
                new RelayCommand(RemoveImageFromAlbum);

            EnterCreateAlbumModeCommand =
                new RelayCommand(EnterCreateAlbumMode);

            SaveAlbumCommand =
                new RelayCommand(SaveAlbum);

            SaveUploadedImageCommand =
                new RelayCommand(() => TrySavePendingImage());

            CancelAddImageCommand =
                new RelayCommand(CancelAddImage);
        }

        public void AddGalleryItem(GalleryItem newGalleryItem)
        {
            DataBaseUtil.AddGalleryItem(newGalleryItem);

            GalleryItems.Add(newGalleryItem);
        }

        public void ReloadData(
            int? selectedAlbumId = null,
            int? selectedGalleryItemId = null)
        {
            GalleryItems = new ObservableCollection<GalleryItem>(
                DataBaseUtil.GetGalleryItems());

            Albums = new ObservableCollection<Album>(
                DataBaseUtil.GetAlbums());

            if (selectedAlbumId.HasValue)
            {
                SelectedAlbum = Albums.FirstOrDefault(
                    a => a.Id == selectedAlbumId.Value);
            }

            if (selectedGalleryItemId.HasValue)
            {
                SelectedGalleryItem = GalleryItems.FirstOrDefault(
                    g => g.Id == selectedGalleryItemId.Value);
            }
        }

        public void SetPendingImagePath(string filePath)
        {
            PendingImagePath = filePath;
        }

        public bool TrySavePendingImage()
        {
            if (SelectedAlbum == null)
                return false;

            if (string.IsNullOrWhiteSpace(PendingImagePath))
                return false;

            if (!File.Exists(PendingImagePath))
                return false;

            var importedPath = ImportImageFile(PendingImagePath);

            var galleryItem = new GalleryItem
            {
                FilePath = importedPath,
                Description = PendingImageDescription.Trim(),
                AddedAt = DateTime.UtcNow
            };

            DataBaseUtil.AddGalleryItem(galleryItem);

            DataBaseUtil.AddImageToAlbum(
                SelectedAlbum.Id,
                galleryItem.Id);

            ReloadData(
                selectedAlbumId: SelectedAlbum.Id,
                selectedGalleryItemId: galleryItem.Id);

            ResetPendingImage();
            IsAddingImage = false;
            IsInsideAlbum = true;

            return true;
        }

        private void EnterCreateAlbumMode()
        {
            IsCreatingAlbum = true;

            NewAlbumTitle = string.Empty;
        }

        private void SaveAlbum()
        {
            CreateAlbum();
            IsCreatingAlbum = false;
        }

        private void CreateAlbum()
        {
            var albumTitle = string.IsNullOrWhiteSpace(NewAlbumTitle)
                ? $"Album {Albums.Count + 1}"
                : NewAlbumTitle.Trim();

            var album = new Album
            {
                Title = albumTitle
            };

            DataBaseUtil.AddAlbum(album);

            Albums.Add(album);

            SelectedAlbum = album;
            NewAlbumTitle = string.Empty;
        }

        private void DeleteAlbum()
        {
            if (SelectedAlbum == null)
                return;

            DataBaseUtil.DeleteAlbum(SelectedAlbum.Id);

            Albums.Remove(SelectedAlbum);

            SelectedAlbum = null;
        }

        private void AddImageToAlbum()
        {
            if (SelectedAlbum == null)
                return;

            if (SelectedGalleryItem == null)
                return;

            DataBaseUtil.AddImageToAlbum(
                SelectedAlbum.Id,
                SelectedGalleryItem.Id);

            if (!SelectedAlbum.Contents.Any(
                    g => g.Id == SelectedGalleryItem.Id))
            {
                SelectedAlbum.Contents.Add(
                    SelectedGalleryItem);
            }
        }

        private void RemoveImageFromAlbum()
        {
            if (SelectedAlbum == null)
                return;

            if (SelectedGalleryItem == null)
                return;

            DataBaseUtil.RemoveImageFromAlbum(
                SelectedAlbum.Id,
                SelectedGalleryItem.Id);

            var image = SelectedAlbum.Contents
                .FirstOrDefault(g =>
                    g.Id == SelectedGalleryItem.Id);

            if (image != null)
            {
                SelectedAlbum.Contents.Remove(image);
            }
        }

        private void CancelAddImage()
        {
            ResetPendingImage();
            IsAddingImage = false;
        }

        private void ResetPendingImage()
        {
            PendingImagePath = string.Empty;
            PendingImageDescription = string.Empty;
        }

        private static string ImportImageFile(string sourcePath)
        {
            var extension = Path.GetExtension(sourcePath);

            if (!SupportedImageExtensions.Contains(
                    extension,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Unsupported image file type.");
            }

            var galleryDirectory = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "gnezdece",
                "gallery");

            Directory.CreateDirectory(galleryDirectory);

            var destinationPath = Path.Combine(
                galleryDirectory,
                $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}");

            File.Copy(sourcePath, destinationPath, overwrite: false);

            return destinationPath;
        }
    }
}