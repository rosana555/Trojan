using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Trojan.Core.Models;

namespace Trojan.Services.Factories
{
    internal class GallaryFactory
    {
        private static readonly string[] ImagePaths = new[]
        {
            "UI/Assets/TestImages/image1.jpg",
            "UI/Assets/TestImages/image2.jpg",
            "UI/Assets/TestImages/image3.jpg",
            "UI/Assets/TestImages/image4.jpg"
        };

        private static readonly string[] ImageDescriptions = new[]
        {
            "Beautiful mountain sunset with golden hour lighting",
            "Amazing beach vacation photo from summer 2024",
            "Team photo from annual company gathering",
            "Conference keynote presentation hall",
            "Panoramic view of our new office space",
            "Latte art from the new coffee shop downtown",
            "City skyline at night with all lights on",
            "Scenic nature trail through the forest"
        };

        private static readonly string[] AlbumTitles = new[]
        {
            "Vacation",
            "Favorites",
            "Work",
            "Nature",
            "Family",
            "Random",
            "Travel",
            "Memories"
        };

        // =========================
        // GENERATE GALLERY ITEMS
        // =========================

        public static List<GalleryItem> Generate(int count)
        {
            var faker = new Faker<GalleryItem>("en_US")
                .RuleFor(g => g.Id, f => 0)
                .RuleFor(g => g.FilePath, f => f.PickRandom(ImagePaths))
                .RuleFor(g => g.Description, f => f.PickRandom(ImageDescriptions))
                .RuleFor(g => g.AddedAt,
                    f => f.Date.Past(1).ToUniversalTime());

            return faker.Generate(count);
        }

        // =========================
        // GENERATE ALBUMS
        // =========================

        public static List<Album> GenerateAlbums(
            int albumCount,
            List<GalleryItem> galleryItems)
        {
            var faker = new Faker("en_US");

            var albums = new List<Album>();

            for (int i = 0; i < albumCount; i++)
            {
                var album = new Album
                {
                    Title = faker.PickRandom(AlbumTitles),

                    Created = faker.Date.Past(1).ToUniversalTime(),

                    LastUpdated = DateTime.UtcNow
                };

                // RANDOM IMAGES
                var randomImages = galleryItems
                    .OrderBy(x => Guid.NewGuid())
                    .Take(faker.Random.Int(2, 6))
                    .ToList();

                foreach (var image in randomImages)
                {
                    album.Contents.Add(image);

                    // MANY TO MANY
                    image.Albums.Add(album);
                }

                // THUMBNAIL
                album.Thumbnail = album.Contents.FirstOrDefault();

                albums.Add(album);
            }

            return albums;
        }
    }
}