using System;
using Trojan.ViewModels;

namespace Trojan.Models
{
    public class Note : ObservableObject
    {
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private DateTime _createdAt = DateTime.UtcNow;
        public DateTime CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }

        private string _title = string.Empty;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private string _content = string.Empty;
        public string Content { get => _content; set => SetProperty(ref _content, value); }

        private DateTime? _editedAt;
        public DateTime? EditedAt { get => _editedAt; set => SetProperty(ref _editedAt, value); }

        private bool _isPinned;
        public bool IsPinned { get => _isPinned; set => SetProperty(ref _isPinned, value); }
    }
}
