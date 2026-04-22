using System.ComponentModel.DataAnnotations;
using Trojan.ViewModels;

namespace Trojan.Models;

public class Joke : ObservableObject
{
    private int _id;
    public int Id { get => _id; set => SetProperty(ref _id, value); }

    private string _content = string.Empty;
    public string Content { get => _content; set => SetProperty(ref _content, value); }

    private JokeCategory _category = JokeCategory.Other;
    public JokeCategory Category { get => _category; set => SetProperty(ref _category, value); }

    private int _rating = 1;
    [Range(1, 5)]
    public int Rating { get => _rating; set => SetProperty(ref _rating, value); }
}