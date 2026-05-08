using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static List<GalleryItem> Generate(int count)
        {
            var faker = new Faker<GalleryItem>("en_US")
                .RuleFor(g => g.Id, f => 0)
                .RuleFor(g => g.FilePath, f => f.PickRandom(ImagePaths))
                .RuleFor(g => g.Description, f => f.PickRandom(ImageDescriptions))
                .RuleFor(g => g.AddedAt, f => f.Date.Past(1).ToUniversalTime());

            return faker.Generate(count);
        }
    }
}
