using System;
using System.Collections.Generic;
using System.Linq;
using Trojan.Models;

namespace Trojan.DataBase
{
    public static class DataBaseUtil
    {
        // Getters
        public static List<Note> GetNotes()
        {
            using var db = new AppDbContext();
            return db.Notes
                .OrderByDescending(n => n.EditedAt ?? n.CreatedAt)
                .ToList();
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

        // Add - function add to db 
        public static void AddNote(Note note)
        {
            using var db = new AppDbContext();
            note.CreatedAt = DateTime.UtcNow;
            note.EditedAt = null;
            db.Notes.Add(note);
            db.SaveChanges();
        }

        public static void SaveNote(Note note)
        {
            using var db = new AppDbContext();
            if (note.Id == 0)
            {
                note.CreatedAt = DateTime.UtcNow;
                note.EditedAt = null;
                db.Notes.Add(note);
            }
            else
            {
                note.EditedAt = DateTime.UtcNow;
                db.Notes.Update(note);
            }

            db.SaveChanges();
        }

        public static void DeleteNote(int noteId)
        {
            using var db = new AppDbContext();
            var note = db.Notes.FirstOrDefault(n => n.Id == noteId);
            if (note is null)
            {
                return;
            }

            db.Notes.Remove(note);
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