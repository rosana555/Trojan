using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trojan.Data.DataBase;
using Trojan.Services.Factories;

namespace Trojan.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext db)
        {
            var galleryItems = GallaryFactory.Generate(10);

            db.Notes.AddRange(NoteFactory.Generate(10));
            db.CalendarEvents.AddRange(CalendarEventFactory.Generate(10));
            db.Jokes.AddRange(JokeFactory.Generate(10));
            db.GalleryItems.AddRange(galleryItems);
            db.Albums.AddRange(
                GallaryFactory.GenerateAlbums(4, galleryItems));

            db.SaveChanges();
        }

        public static void SeedAlbums(AppDbContext db)
        {
            if (db.Albums.Any())
                return;

            var galleryItems = db.GalleryItems.ToList();

            if (!galleryItems.Any())
                return;

            db.Albums.AddRange(
                GallaryFactory.GenerateAlbums(4, galleryItems));

            db.SaveChanges();
        }
    }
}