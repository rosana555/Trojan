namespace Trojan.Models;

using System;
using Trojan.ViewModels;

public class GalleryItem : ObservableObject
{
    private int _id;
    public int Id { get => _id; set => SetProperty(ref _id, value); }

    private string _filePath = string.Empty;
    public string FilePath { get => _filePath; set => SetProperty(ref _filePath, value); }

    private string _description = string.Empty;
    public string Description { get => _description; set => SetProperty(ref _description, value); }

    private DateTime _addedAt = DateTime.UtcNow;
    public DateTime AddedAt { get => _addedAt; set => SetProperty(ref _addedAt, value); }
}