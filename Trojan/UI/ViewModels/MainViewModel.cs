using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Trojan.Data.DataBase;
using Trojan.Core.Models;
using Trojan.Core.Base;

namespace Trojan.UI.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        // OBSERVABLE COLLECTIONS
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

        // LOAD DATA 
        private void LoadData()
        {
            using var db = new AppDbContext();

            Notes = new ObservableCollection<Note>(db.Notes.ToList());
            Jokes = new ObservableCollection<Joke>(db.Jokes.ToList());
            CalendarEvents = new ObservableCollection<CalendarEvent>(db.CalendarEvents.ToList());
            GalleryItems = new ObservableCollection<GalleryItem>(db.GalleryItems.ToList());
        }

        // ADD METHODS 
        public void AddNote(Note newNote)
        {
            using var db = new AppDbContext();

            db.Notes.Add(newNote);
            db.SaveChanges();

            Notes.Add(newNote);
        }

        public void AddJoke(Joke newJoke)
        {
            using var db = new AppDbContext();

            db.Jokes.Add(newJoke);
            db.SaveChanges();

            Jokes.Add(newJoke);
        }

        public void AddCalendarEvent(CalendarEvent newCalendarEvent)
        {
            using var db = new AppDbContext();

            db.CalendarEvents.Add(newCalendarEvent);
            db.SaveChanges();

            CalendarEvents.Add(newCalendarEvent);
        }

        public void AddGalleryItem(GalleryItem newGalleryItem)
        {
            using var db = new AppDbContext();

            db.GalleryItems.Add(newGalleryItem);
            db.SaveChanges();

            GalleryItems.Add(newGalleryItem);
        }
    }
}