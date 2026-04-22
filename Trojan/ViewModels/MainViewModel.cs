namespace Trojan.ViewModels;

using System.Collections.ObjectModel;
using Trojan.Models;

public class MainViewModel : ObservableObject
{
    private ObservableCollection<Note> _notes = new();
    public ObservableCollection<Note> Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    private ObservableCollection<CalendarEvent> _calendarEvents = new();
    public ObservableCollection<CalendarEvent> CalendarEvents
    {
        get => _calendarEvents;
        set => SetProperty(ref _calendarEvents, value);
    }

    private ObservableCollection<GalleryItem> _galleryItems = new();
    public ObservableCollection<GalleryItem> GalleryItems
    {
        get => _galleryItems;
        set => SetProperty(ref _galleryItems, value);
    }

    private ObservableCollection<Joke> _jokes = new();
    public ObservableCollection<Joke> Jokes
    {
        get => _jokes;
        set => SetProperty(ref _jokes, value);
    }

    public MainViewModel()
    {
    }
}