using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trojan.Data.Seeders;
using Trojan.Data.DataBase;
using Trojan.Services.Logger;
namespace Trojan;

public class AppBootstrapper
{
    public void Run()
    {
        using var db = new AppDbContext();
        //comment if you dont want your db to be deleted each app launch 
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        if(db.Notes.Count() <= 10)
        {
            DatabaseSeeder.Seed(db);
        }

        AppLog.Info($"Notes loaded: {db.Notes.Count()}");
        if (db.Notes.Any())
        {
            var note = db.Notes.First();
            AppLog.Info($"  Sample: Title={note.Title}, Content={note.Content}, IsPinned={note.IsPinned}, CreatedAt={note.CreatedAt}");
        }

        AppLog.Info($"CalendarEvents loaded: {db.CalendarEvents.Count()}");
        if (db.CalendarEvents.Any())
        {
            var calEvent = db.CalendarEvents.First();
            AppLog.Info($"  Sample: Title={calEvent.Title}, Description={calEvent.Description}, Location={calEvent.Location}, StartDateTime={calEvent.StartDateTime}, EndDateTime={calEvent.EndDateTime}, ColorHex={calEvent.ColorHex}");
        }

        AppLog.Info($"Jokes loaded: {db.Jokes.Count()}");
        if (db.Jokes.Any())
        {
            var joke = db.Jokes.First();
            AppLog.Info($"  Sample: Content={joke.Content}, Category={joke.Category}, Rating={joke.Rating}");
        }

        AppLog.Info($"GalleryItems loaded: {db.GalleryItems.Count()}");
        if (db.GalleryItems.Any())
        {
            var gallery = db.GalleryItems.First();
            AppLog.Info($"  Sample: FilePath={gallery.FilePath}, Description={gallery.Description}, AddedAt={gallery.AddedAt}");
        }
    }
}