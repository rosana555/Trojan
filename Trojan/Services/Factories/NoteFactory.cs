using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Trojan.Core.Models;

namespace Trojan.Services.Factories
{
    internal class NoteFactory
    {
        private static readonly string[] NoteTitles = new[]
        {
            "Shopping List",
            "Meeting Notes",
            "Project Ideas",
            "Book Recommendations",
            "Fitness Goals",
            "Travel Plans",
            "Recipe Ideas",
            "Bug Fix Reminder",
            "Code Review Points",
            "Team Feedback",
            "Learning Resources",
            "Daily Tasks"
        };

        private static readonly string[] NoteContents = new[]
        {
            "Remember to buy milk, eggs, bread, and cheese from the store. Also pick up coffee beans.",
            "Discussed Q4 roadmap. Need to implement new authentication system. Follow up with backend team.",
            "1. Build real-time dashboard\n2. Improve API response time\n3. Add dark mode to UI\n4. Refactor database queries",
            "The Midnight Library - Matt Haig\nAtomic Habits - James Clear\nDeep Work - Cal Newport",
            "Monday: 5km run, Wednesday: Gym (chest day), Friday: Yoga session. Aim for 10k steps daily.",
            "July trip to Croatia. Check flights for June 15-22. Book Airbnb in Split. Reserve rental car.",
            "Pasta Carbonara: eggs, guanciale, pecorino, black pepper. Spaghetti Aglio e Olio: garlic, chili, parsley.",
            "Fix null reference exception in UserController. Add proper error handling and logging.",
            "1. Code readability needs improvement on line 45\n2. Add unit tests for new endpoint\n3. Update API documentation",
            "Great collaboration on sprint planning. Sarah's ideas were innovative. John needs more mentoring.",
            "Pluralsight courses on microservices architecture. YouTube channel on system design.",
            "Complete code review, Send report to manager, Team standup at 2pm, Update Jira tickets"
        };

        public static List<Note> Generate(int count)
        {
            var faker = new Faker<Note>("en_US")
                .RuleFor(n => n.Id, f => 0)
                .RuleFor(n => n.CreatedAt,
                    f => f.Date.Past(1).ToUniversalTime())
                .RuleFor(n => n.Title,
                    f => f.PickRandom(NoteTitles))
                .RuleFor(n => n.Content,
                    f => f.PickRandom(NoteContents))
                .RuleFor(n => n.EditedAt,
                    (f, n) => f.Random.Bool(0.6f)
                        ? n.CreatedAt.AddHours(f.Random.Int(1, 72))
                        : null)
                .RuleFor(n => n.IsPinned,
                    f => f.Random.Bool(0.15f));

            return faker.Generate(count);
        }
    }
}