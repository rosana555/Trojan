using System;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                .AsNoTracking()
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.EditedAt ?? n.CreatedAt)
                .ToList();
        }

        public static List<Joke> GetJokes()
        {
            using var db = new AppDbContext();
            return db.Jokes
                .AsNoTracking()
                .ToList();
        }

        public static List<CalendarEvent> GetCalendarEvents()
        {
            using var db = new AppDbContext();
            return db.CalendarEvents
                .AsNoTracking()
                .ToList();
        }

        public static List<GalleryItem> GetGalleryItems()
        {
            using var db = new AppDbContext();
            return db.GalleryItems
                .AsNoTracking()
                .ToList();
        }

        // Add - function add to db
        public static void AddNote(Note note)
        {
            using var db = new AppDbContext();
            note.CreatedAt = DateTime.Now;
            note.EditedAt = null;
            db.Notes.Add(note);
            db.SaveChanges();
        }

        public static void SaveNote(Note note)
        {
            using var db = new AppDbContext();

            if (note.Id == 0)
            {
                note.CreatedAt = DateTime.Now;
                note.EditedAt = null;

                db.Notes.Add(note);
                db.SaveChanges();
                return;
            }

            note.EditedAt = DateTime.Now;

            db.Attach(note);

            db.Entry(note).Property(n => n.Title).IsModified = true;
            db.Entry(note).Property(n => n.Content).IsModified = true;
            db.Entry(note).Property(n => n.EditedAt).IsModified = true;
            db.Entry(note).Property(n => n.IsPinned).IsModified = true;

            db.Entry(note).Property(n => n.CreatedAt).IsModified = false;

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

        public static   void AddJoke(Joke joke)
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