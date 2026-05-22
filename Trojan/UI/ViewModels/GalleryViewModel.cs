using System.Collections.ObjectModel;
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

        public GalleryViewModel()
        {
            GalleryItems = new ObservableCollection<GalleryItem>(
                DataBaseUtil.GetGalleryItems());

            Albums = new ObservableCollection<Album>(
                DataBaseUtil.GetAlbums());

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
        }

        public void AddGalleryItem(GalleryItem newGalleryItem)
        {
            DataBaseUtil.AddGalleryItem(newGalleryItem);

            GalleryItems.Add(newGalleryItem);
        }

        private void EnterCreateAlbumMode()
        {
            IsCreatingAlbum = true;

            NewAlbumTitle = string.Empty;
        }

        private void SaveAlbum()
        {
            IsCreatingAlbum = false;
        }

        private void CreateAlbum()
        {
            var album = new Album
            {
                Title = "New Album"
            };

            DataBaseUtil.AddAlbum(album);

            Albums.Add(album);

            SelectedAlbum = album;
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
    }
}