namespace Trojan.ViewModels;

using System.Collections.ObjectModel;
using Trojan.DataBase;
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
        LoadData();
    }
    //loads data from db to UI
    private void LoadData()
    {
        Notes = new ObservableCollection<Note>(DataBaseUtil.GetNotes());
        Jokes = new ObservableCollection<Joke>(DataBaseUtil.GetJokes());    
        CalendarEvents = new ObservableCollection<CalendarEvent>(DataBaseUtil.GetCalendarEvents());
        GalleryItems = new ObservableCollection<GalleryItem>(DataBaseUtil.GetGalleryItems());
    }
    // Adding functions 
    public void AddNote(Note newNote)
    {
        DataBaseUtil.AddNote(newNote);
        Notes.Add(newNote);
    }
    public void AddJoke(Joke newJoke)
    {
        DataBaseUtil.AddJoke(newJoke);
        Jokes.Add(newJoke);
    }
    public void AddCalendarEvent(CalendarEvent newCalendarEvent)
    {
        DataBaseUtil.AddCalendarEvent(newCalendarEvent);
        CalendarEvents.Add(newCalendarEvent);
    }
    public void AddGalleryItem(GalleryItem newGalleryItem)
    {
        DataBaseUtil.AddGalleryItem(newGalleryItem);
        GalleryItems.Add(newGalleryItem);
    }

}