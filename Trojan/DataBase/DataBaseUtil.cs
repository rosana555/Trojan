using System.Collections.Generic;
using System.Linq;
using Trojan.Models;

namespace Trojan.DataBase
{
    public static class DataBaseUtil
    {
        // --- GET ALL ---
        public static List<Note> GetNotes()
        {
            using var db = new AppDbContext();
            return db.Notes.ToList();
        }

        public static List<Joke> GetJokes()
        {
            using var db = new AppDbContext();
            return db.Jokes.ToList();
        }

        public static List<CalendarEvent> GetCalendarEvents()
        {
            using var db = new AppDbContext();
            return db.CalendarEvents.ToList();
        }

        public static List<GalleryItem> GetGalleryItems()
        {
            using var db = new AppDbContext();
            return db.GalleryItems.ToList();
        }

        // --- ADD ---
        public static void AddNote(Note note)
        {
            using var db = new AppDbContext();
            db.Notes.Add(note);
            db.SaveChanges();
        }

        public static void AddJoke(Joke joke)
        {
            using var db = new AppDbContext();
            db.Jokes.Add(joke);
            db.SaveChanges();
        }

        public static void AddCalendarEvent(CalendarEvent calendarEvent)
        {
            using var db = new AppDbContext();
            db.CalendarEvents.Add(calendarEvent);
            db.SaveChanges();
        }

        public static void AddGalleryItem(GalleryItem galleryItem)
        {
            using var db = new AppDbContext();
            db.GalleryItems.Add(galleryItem);
            db.SaveChanges();
        }
    }
}