using System;
using System.Collections.ObjectModel;
using Trojan.Core.Base;

namespace Trojan.Core.Models
{
    public class Album : ObservableObject
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private GalleryItem _thumbnail;
        public GalleryItem Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }


        private ObservableCollection<GalleryItem> _contents;

        public ObservableCollection<GalleryItem> Contents
        {
            get => _contents;
            set => SetProperty(ref _contents, value);
        }

        private DateTime _created = DateTime.UtcNow;
        public DateTime Created
        {
            get => _created;
            set => SetProperty(ref _created, value);
        }

        private DateTime _lastUpdated = DateTime.UtcNow;
        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        public Album()
        {
            _contents = new ObservableCollection<GalleryItem>();
        }
    }
}