using System.ComponentModel.DataAnnotations;
using Trojan.Core.Base;

namespace Trojan.Core.Models;

public class Fact : ObservableObject
{
    private int _id;
    public int Id { get => _id; set => SetProperty(ref _id, value); }

    private string _content = string.Empty;
    public string Content { get => _content; set => SetProperty(ref _content, value); }
    
    private int _rating = 1;
    [Range(1, 5)]
    public int Rating { get => _rating; set => SetProperty(ref _rating, value); }
}