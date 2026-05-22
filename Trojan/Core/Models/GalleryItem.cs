using System;
using System.Collections.ObjectModel;
using Trojan.Core.Base;

namespace Trojan.Core.Models
{
    public class GalleryItem : ObservableObject
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _filePath = string.Empty;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private DateTime _addedAt = DateTime.UtcNow;
        public DateTime AddedAt
        {
            get => _addedAt;
            set => SetProperty(ref _addedAt, value);
        }


        private ObservableCollection<Album> _albums;

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set => SetProperty(ref _albums, value);
        }

        public GalleryItem()
        {
            _albums = new ObservableCollection<Album>();
        }
    }
}