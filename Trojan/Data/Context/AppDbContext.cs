using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Trojan.Core.Models;

namespace Trojan.Data.DataBase
{
    public class AppDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }
        public DbSet<Joke> Jokes { get; set; }
        public DbSet<Album> Albums { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = GetDatabasePath();

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<Album>()
                .HasMany(a => a.Contents)
                .WithMany(g => g.Albums);


            modelBuilder.Entity<Album>()
                .HasOne(a => a.Thumbnail)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
        }

        private static string GetDatabasePath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "gnezdece");

            Directory.CreateDirectory(dir);

            return Path.Combine(dir, "trojan.db");
        }
        
        public void EnsureRemindersTable()
        {
            Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS Reminders (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL DEFAULT '',
            TriggerAt TEXT NOT NULL DEFAULT '2000-01-01',
            Recurrence INTEGER NOT NULL DEFAULT 0,
            LinkedEventId INTEGER NULL,
            IsTriggered INTEGER NOT NULL DEFAULT 0
        );
    ");
        }
    }
}