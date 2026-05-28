using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Trojan.Core.Models;

namespace Trojan.Data.DataBase
{
    public static class DataBaseUtil
    {
        private static AppDbContext CreateDbContext()
        {
            var db = new AppDbContext();

            db.Database.EnsureCreated();

            return db;
        }


        public static List<Note> GetNotes()
        {
            using var db = CreateDbContext();

            return db.Notes
                .AsNoTracking()
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.EditedAt ?? n.CreatedAt)
                .ToList();
        }

        public static void AddNote(Note note)
        {
            if (note is null)
                throw new ArgumentNullException(nameof(note));

            using var db = CreateDbContext();

            db.Notes.Add(note);

            db.SaveChanges();
        }

        public static void SaveNote(Note note)
        {
            if (note is null)
                throw new ArgumentNullException(nameof(note));

            using var db = CreateDbContext();

            note.EditedAt = DateTime.Now;

            db.Notes.Update(note);

            db.SaveChanges();
        }

        public static void DeleteNote(int id)
        {
            using var db = CreateDbContext();

            var entity = new Note { Id = id };

            db.Notes.Attach(entity);

            db.Notes.Remove(entity);

            db.SaveChanges();
        }


        public static List<Joke> GetJokes()
        {
            using var db = CreateDbContext();

            return db.Jokes
                .AsNoTracking()
                .ToList();
        }

        public static void AddJoke(Joke joke)
        {
            if (joke is null)
                throw new ArgumentNullException(nameof(joke));

            using var db = CreateDbContext();

            db.Jokes.Add(joke);

            db.SaveChanges();
        }

        public static List<CalendarEvent> GetCalendarEvents()
        {
            using var db = CreateDbContext();

            return db.CalendarEvents
                .AsNoTracking()
                .ToList();
        }

        public static void AddCalendarEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent is null)
                throw new ArgumentNullException(nameof(calendarEvent));

            using var db = CreateDbContext();

            db.CalendarEvents.Add(calendarEvent);

            db.SaveChanges();
        }

        public static List<GalleryItem> GetGalleryItems()
        {
            using var db = CreateDbContext();

            return db.GalleryItems
                .Include(g => g.Albums)
                .AsNoTracking()
                .ToList();
        }

        public static void AddGalleryItem(GalleryItem galleryItem)
        {
            if (galleryItem is null)
                throw new ArgumentNullException(nameof(galleryItem));

            using var db = CreateDbContext();

            db.GalleryItems.Add(galleryItem);

            db.SaveChanges();
        }

        public static List<Album> GetAlbums()
        {
            using var db = CreateDbContext();

            return db.Albums
                .Include(a => a.Contents)
                .Include(a => a.Thumbnail)
                .AsNoTracking()
                .OrderByDescending(a => a.LastUpdated)
                .ToList();
        }

        public static void AddAlbum(Album album)
        {
            if (album is null)
                throw new ArgumentNullException(nameof(album));

            using var db = CreateDbContext();

            db.Albums.Add(album);

            db.SaveChanges();
        }

        public static void DeleteAlbum(int albumId)
        {
            using var db = CreateDbContext();

            var album = db.Albums
                .Include(a => a.Contents)
                .FirstOrDefault(a => a.Id == albumId);

            if (album == null)
                return;

            album.Contents.Clear();

            db.Albums.Remove(album);

            db.SaveChanges();
        }

        public static void AddImageToAlbum(int albumId, int galleryItemId)
        {
            using var db = CreateDbContext();

            var album = db.Albums
                .Include(a => a.Contents)
                .FirstOrDefault(a => a.Id == albumId);

            var image = db.GalleryItems
                .FirstOrDefault(g => g.Id == galleryItemId);

            if (album == null || image == null)
                return;

            if (!album.Contents.Any(g => g.Id == image.Id))
            {
                album.Contents.Add(image);
            }

            if (album.Thumbnail == null)
            {
                album.Thumbnail = image;
            }

            album.LastUpdated = DateTime.UtcNow;

            db.SaveChanges();
        }

        public static void RemoveImageFromAlbum(int albumId, int galleryItemId)
        {
            using var db = CreateDbContext();

            var album = db.Albums
                .Include(a => a.Contents)
                .Include(a => a.Thumbnail)
                .FirstOrDefault(a => a.Id == albumId);

            if (album == null)
                return;

            var image = album.Contents
                .FirstOrDefault(g => g.Id == galleryItemId);

            if (image == null)
                return;

            album.Contents.Remove(image);

            if (album.Thumbnail?.Id == image.Id)
            {
                album.Thumbnail = album.Contents.FirstOrDefault();
            }

            album.LastUpdated = DateTime.UtcNow;

            db.SaveChanges();
        }
    }
}