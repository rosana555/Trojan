using System.Linq;
using Microsoft.EntityFrameworkCore;
using Trojan.Data.DataBase;
using Trojan.Data.Seeders;
using Trojan.Services.Logger;

namespace Trojan;

public class AppBootstrapper
{
    public void Run()
    {
        using var db = new AppDbContext();

        var isNewDatabase = db.Database.EnsureCreated();

        EnsureGallerySchema(db);

        if (isNewDatabase)
        {
            DatabaseSeeder.Seed(db);
        }
        else if (!db.Albums.Any() && db.GalleryItems.Any())
        {
            DatabaseSeeder.SeedAlbums(db);
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

    private static void EnsureGallerySchema(AppDbContext db)
    {
        if (!NeedsAlbumThumbnailMigration(db))
            return;

        db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
        try
        {
            using var transaction = db.Database.BeginTransaction();

            db.Database.ExecuteSqlRaw(
                """
                CREATE TABLE "Albums_tmp" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Albums_tmp" PRIMARY KEY AUTOINCREMENT,
                    "ThumbnailId" INTEGER NULL,
                    "Title" TEXT NOT NULL,
                    "Created" TEXT NOT NULL,
                    "LastUpdated" TEXT NOT NULL,
                    CONSTRAINT "FK_Albums_tmp_GalleryItems_ThumbnailId"
                        FOREIGN KEY ("ThumbnailId")
                        REFERENCES "GalleryItems" ("Id")
                        ON DELETE SET NULL
                );
                """);

            db.Database.ExecuteSqlRaw(
                """
                INSERT INTO "Albums_tmp" ("Id", "ThumbnailId", "Title", "Created", "LastUpdated")
                SELECT "Id", "ThumbnailId", "Title", "Created", "LastUpdated"
                FROM "Albums";
                """);

            db.Database.ExecuteSqlRaw("""DROP TABLE "Albums";""");
            db.Database.ExecuteSqlRaw(
                """ALTER TABLE "Albums_tmp" RENAME TO "Albums";""");
            db.Database.ExecuteSqlRaw(
                """CREATE INDEX "IX_Albums_ThumbnailId" ON "Albums" ("ThumbnailId");""");

            transaction.Commit();
        }
        finally
        {
            db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
        }
    }

    private static bool NeedsAlbumThumbnailMigration(AppDbContext db)
    {
        using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "PRAGMA table_info(Albums);";

        var connection = command.Connection
            ?? throw new InvalidOperationException(
                "Database connection is not available.");

        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var columnName = reader.GetString(1);
            if (!string.Equals(
                    columnName,
                    "ThumbnailId",
                    System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return reader.GetInt32(3) == 1;
        }

        return false;
    }
}