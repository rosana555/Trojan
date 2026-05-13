using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Trojan.Core.Models;

namespace Trojan.Data.DataBase;

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

    public static List<Joke> GetJokes()
    {
        using var db = CreateDbContext();
        return db.Jokes.AsNoTracking().ToList();
    }

    public static List<CalendarEvent> GetCalendarEvents()
    {
        using var db = CreateDbContext();
        return db.CalendarEvents.AsNoTracking().ToList();
    }

    public static List<GalleryItem> GetGalleryItems()
    {
        using var db = CreateDbContext();
        return db.GalleryItems.AsNoTracking().ToList();
    }

    public static void AddNote(Note note)
    {
        if (note is null) throw new ArgumentNullException(nameof(note));

        using var db = CreateDbContext();
        db.Notes.Add(note);
        db.SaveChanges();
    }

    public static void SaveNote(Note note)
    {
        if (note is null) throw new ArgumentNullException(nameof(note));

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

    public static void AddJoke(Joke joke)
    {
        if (joke is null) throw new ArgumentNullException(nameof(joke));

        using var db = CreateDbContext();
        db.Jokes.Add(joke);
        db.SaveChanges();
    }

    public static void AddCalendarEvent(CalendarEvent calendarEvent)
    {
        if (calendarEvent is null) throw new ArgumentNullException(nameof(calendarEvent));

        using var db = CreateDbContext();
        db.CalendarEvents.Add(calendarEvent);
        db.SaveChanges();
    }

    public static void AddGalleryItem(GalleryItem galleryItem)
    {
        if (galleryItem is null) throw new ArgumentNullException(nameof(galleryItem));

        using var db = CreateDbContext();
        db.GalleryItems.Add(galleryItem);
        db.SaveChanges();
    }
}
