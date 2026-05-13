using System;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Trojan.Models;

namespace Trojan.DataBase
{
    public class AppDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }
        public DbSet<Joke> Jokes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = GetDatabasePath();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        private static string GetDatabasePath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "gnezdece");

            Directory.CreateDirectory(dir);

            return Path.Combine(dir, "trojan.db");
        }
    }
}
